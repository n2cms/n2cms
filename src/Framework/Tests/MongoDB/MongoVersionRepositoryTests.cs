using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Tests.Persistence;
using NUnit.Framework;
using N2.Edit.Versioning;
using Shouldly;
using N2.Persistence;
using N2.Definitions.Static;
using N2.Edit.Workflow;
using N2.Web;
using N2.Tests.Fakes;
using N2.Tests.Edit.Items;
using N2.Persistence.MongoDB;

namespace N2.Tests.MongoDB
{
    [TestFixture]
    public class MongoVersionRepositoryTests : ItemTestsBase
    {
        ContentVersionRepository repository;
        IPersister persister;
        ContentActivator activator;
        private DraftRepository drafts;
        private MongoContentItemRepository itemRepository;
        private MongoDatabaseProvider databaseProvider;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var definitionProviders = TestSupport.SetupDefinitionProviders(new DefinitionMap(), typeof(NormalPage), typeof(NormalItem));
            var proxies = new N2.Persistence.Proxying.InterceptingProxyFactory();
            proxies.Initialize(definitionProviders.SelectMany(dp => dp.GetDefinitions()));

            itemRepository = new MongoContentItemRepository(
                databaseProvider = new MongoDatabaseProvider(TestSupport.CreateDependencyInjector(), proxies, new N2.Configuration.ConfigurationManagerWrapper("n2mongo"),
                definitionProviders,
                new AdaptiveContext()));

            persister = new ContentPersister(TestSupport.SetupContentSource(itemRepository), itemRepository);
            IRepository<ContentVersion> versionRepository = new MongoDbRepository<ContentVersion>(databaseProvider);
            repository = TestSupport.CreateVersionRepository(
                ref persister,
                ref activator,
                ref versionRepository,
                typeof(NormalPage), typeof(NormalItem));
            drafts = new DraftRepository(repository, new FakeCacheWrapper());
        }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            
            itemRepository.Provider.DropDatabases();
        }

        [Test]
        public void MasterVersion_CanBeSavedAsVersion_AndRetrieved()
        {
            var master = CreateOneItem<NormalPage>(0, "pageX", null);
            persister.Save(master);

            var draft = repository.Save(master);
            repository.Repository.Dispose();

            var savedDraft = repository.GetVersion(master);
            savedDraft.Published.ShouldBe(master.Published, TimeSpan.FromSeconds(10));
            //savedDraft.PublishedBy.ShouldBe(master.SavedBy);
            savedDraft.Saved.ShouldBe(N2.Utility.CurrentTime(), TimeSpan.FromSeconds(10));
            savedDraft.SavedBy.ShouldBe(draft.SavedBy);
            savedDraft.State.ShouldBe(master.State);
            savedDraft.VersionIndex.ShouldBe(master.VersionIndex);
            savedDraft.VersionDataXml.ShouldContain("pageX");
        }

        [Test]
        public void Version_CanBeSavedAsVersioin_AndRetrieved()
        {
            var page = CreateOneItem<NormalPage>(0, "page", null);
            persister.Save(page);

            var version = page.Clone(true);
            version.VersionOf = page;

            var draft = repository.Save(version);
            repository.Repository.Dispose();

            var savedDraft = repository.GetVersion(page);
            savedDraft.Master.Value.ShouldBe(page);
        }

        [Test]
        public void VersionWithDraftStatus_CanBeRetrievedAsDraft()
        {
            var page = CreateOneItem<NormalPage>(0, "page", null);
            persister.Save(page);

            var version = page.Clone(true);
            version.State = ContentState.Draft;
            version.VersionOf = page;
            version.VersionIndex++;

            var draft = repository.Save(version);

            drafts.FindDrafts(page).Single().ID.ShouldBe(draft.ID);
        }

        [TestCase(ContentState.Deleted)]
        [TestCase(ContentState.New)]
        [TestCase(ContentState.None)]
        [TestCase(ContentState.Published)]
        [TestCase(ContentState.Unpublished)]
        [TestCase(ContentState.Waiting)]
        public void OtherStates_ArntDrafts(ContentState state)
        {
            var page = CreateOneItem<NormalPage>(0, "page", null);
            persister.Save(page);

            var version = page.Clone(true);
            version.State = state;
            version.VersionOf = page;

            var draft = repository.Save(version);

            drafts.HasDraft(page).ShouldBe(false);
            drafts.FindDrafts(page).ShouldBeEmpty();
        }

        [Test]
        public void MultipleDrafts_GreatestVersionIndex_IsFirst()
        {
            var page = CreateOneItem<NormalPage>(0, "page", null);
            persister.Save(page);

            var version = page.Clone(true);
            version.VersionIndex = page.VersionIndex + 1;
            version.State = ContentState.Draft;
            version.VersionOf = page;
            var draft1 = repository.Save(version);

            var version2 = page.Clone(true);
            version2.VersionIndex = page.VersionIndex + 2;
            version2.State = ContentState.Draft;
            version2.VersionOf = page;
            var draft2 = repository.Save(version2);

            drafts.FindDrafts(page).First().ID.ShouldBe(draft2.ID);
        }

        [Test]
        public void VersionIndex_IsKeptWhenSavingVersion()
        {
            var page = CreateOneItem<NormalPage>(0, "page", null);
            persister.Save(page);

            var versionItem = page.Clone(true);
            versionItem.VersionIndex = page.VersionIndex + 1;
            versionItem.State = ContentState.Draft;
            versionItem.VersionOf = page;
            
            var version = repository.Save(versionItem);

            repository.Repository.Dispose();
            var savedVersion = repository.GetVersion(page, versionItem.VersionIndex);

            savedVersion.VersionIndex.ShouldBe(versionItem.VersionIndex);
            savedVersion.Version.VersionIndex.ShouldBe(versionItem.VersionIndex);
        }

        [Test]
        public void Parts_AreSerialized()
        {
            var page = CreateOneItem<NormalPage>(0, "page", null);
            persister.Save(page);
            var part = CreateOneItem<NormalItem>(0, "part", page);
            part.ZoneName = "TheZone";
            persister.Save(part);

            var version = page.CloneForVersioningRecursive(new StateChanger());
            version.VersionIndex = page.VersionIndex + 1;
            version.VersionOf = page;

            repository.Save(version);

            repository.Repository.Dispose();
            var savedVersion = repository.GetVersion(page, version.VersionIndex);

            var deserializedPage = savedVersion.Version;
            var deserializedPart = deserializedPage.Children.Single();

            deserializedPart.Title.ShouldBe("part");
            deserializedPart.VersionOf.ID.ShouldBe(part.ID);
            deserializedPart.ZoneName.ShouldBe(part.ZoneName);
        }

        [Test]
        public void Details_AreSerialized()
        {
            var page = CreateOneItem<NormalPage>(0, "page", null);
            page["Hello"] = "world";
            page.GetDetailCollection("Stuffs", true).Add("Hello");
            persister.Save(page);

            var version = page.Clone(true);
            version.VersionIndex = page.VersionIndex + 1;
            version.VersionOf = page;

            repository.Save(version);

            repository.Repository.Dispose();
            var savedVersion = repository.GetVersion(page, version.VersionIndex);

            var deserializedPage = savedVersion.Version;
            deserializedPage["Hello"].ShouldBe("world");
            deserializedPage.GetDetailCollection("Stuffs", false)[0].ShouldBe("Hello");
        }

        [Test]
        public void Details_OnParts_AreSerialized()
        {
            var page = CreateOneItem<NormalPage>(0, "page", null);
            persister.Save(page);
            var part = CreateOneItem<NormalItem>(0, "part", page);
            part["Hello"] = "world";
            part.GetDetailCollection("Stuffs", true).Add("Hello");
            part.ZoneName = "TheZone";
            persister.Save(part);

            var version = page.CloneForVersioningRecursive(new StateChanger());
            version.VersionIndex = page.VersionIndex + 1;

            repository.Save(version);

            repository.Repository.Dispose();
            var savedVersion = repository.GetVersion(page, version.VersionIndex);

            var deserializedPage = savedVersion.Version;
            var deserializedPart = deserializedPage.Children.Single();
            deserializedPart["Hello"].ShouldBe("world");
            deserializedPart.GetDetailCollection("Stuffs", false)[0].ShouldBe("Hello");
        }

        [Test]
        public void NotSavedAsPrevious_IsFoundAsDraft()
        {
            var master = CreateOneItem<NormalPage>(0, "pageX", null);
            persister.Save(master);

            var manager = new VersionManager(repository, persister.Repository, new StateChanger(), new N2.Configuration.EditSection());
            manager.AddVersion(master, asPreviousVersion: false);

            drafts.FindDrafts().Single().Master.ID.ShouldBe(master.ID);
        }

        [Test]
        public void SavedAsPrevious_IsNotFoundAsDraft()
        {
            var master = CreateOneItem<NormalPage>(0, "pageX", null);
            persister.Save(master);

            var manager = new VersionManager(repository, persister.Repository, new StateChanger(), new N2.Configuration.EditSection());
            manager.AddVersion(master, asPreviousVersion: true);

            drafts.FindDrafts().Any().ShouldBe(false);
        }

        [Test]
        public void DraftsCanBeFound_ForSingleItem()
        {
            var master = CreateOneItem<NormalPage>(0, "pageX", null);
            persister.Save(master);

            var manager = new VersionManager(repository, persister.Repository, new StateChanger(), new N2.Configuration.EditSection());
            manager.AddVersion(master, asPreviousVersion: false);

            drafts.FindDrafts().Single().Master.ID.ShouldBe(master.ID);
        }

        [Test]
        public void AutoImplementedProperties_AreMaintained()
        {
            var master = CreateOneItem<NormalPage>(0, "master", null);
            master.WidthType = WidthType.Pixels;
            master.Width = 123;
            persister.Save(master);

            var manager = new VersionManager(repository, persister.Repository, new StateChanger(), new N2.Configuration.EditSection());
            var version = (NormalPage)manager.AddVersion(master, asPreviousVersion: false);

            persister.Dispose();

            master = (NormalPage)persister.Get(master.ID);
            version = (NormalPage)manager.GetVersion(master, version.VersionIndex);

            version.WidthType.ShouldBe(WidthType.Pixels);
            version.Width.ShouldBe(123);
        }

        [Test]
        public void AutoImplementedProperties_AreMaintained_OnPersistedItems()
        {
            var master = CreateOneItem<NormalPage>(0, "master", null);
            master.WidthType = WidthType.Pixels;
            master.Width = 123;
            persister.Save(master);

            persister.Dispose();
            master = (NormalPage)persister.Get(master.ID);

            var manager = new VersionManager(repository, persister.Repository, new StateChanger(), new N2.Configuration.EditSection());
            var version = (NormalPage)manager.AddVersion(master, asPreviousVersion: false);

            persister.Dispose();

            master = (NormalPage)persister.Get(master.ID);
            version = (NormalPage)manager.GetVersion(master, version.VersionIndex);

            version.WidthType.ShouldBe(WidthType.Pixels);
            version.Width.ShouldBe(123);
        }

        [Test]
        public void AutoImplementedProperties_AreMaintained_OnParts()
        {
            var master = CreateOneItem<NormalPage>(0, "master", null);
            persister.Save(master);

            var part = CreateOneItem<NormalItem>(0, "part", master);
            part.WidthType = WidthType.Pixels;
            part.Width = 123;
            part.ZoneName = "TheZone";
            persister.Save(part);

            var manager = new VersionManager(repository, persister.Repository, new StateChanger(), new N2.Configuration.EditSection());
            var version = (NormalPage)manager.AddVersion(master, asPreviousVersion: false);

            persister.Dispose();

            master = (NormalPage)persister.Get(master.ID);
            version = (NormalPage)manager.GetVersion(master, version.VersionIndex);

            version.Children.OfType<NormalItem>().Single().WidthType.ShouldBe(WidthType.Pixels);
            version.Children.OfType<NormalItem>().Single().Width.ShouldBe(123);
        }

        [Test]
        public void AutoImplementedProperties_AreMaintained_OnPeristedParts()
        {
            var master = CreateOneItem<NormalPage>(0, "master", null);
            persister.Save(master);

            var part = CreateOneItem<NormalItem>(0, "part", master);
            part.WidthType = WidthType.Pixels;
            part.Width = 123;
            part.ZoneName = "TheZone";
            persister.Save(part);

            persister.Dispose();
            master = (NormalPage)persister.Get(master.ID);

            var manager = new VersionManager(repository, persister.Repository, new StateChanger(), new N2.Configuration.EditSection());
            var version = (NormalPage)manager.AddVersion(master, asPreviousVersion: false);

            persister.Dispose();

            master = (NormalPage)persister.Get(master.ID);
            version = (NormalPage)manager.GetVersion(master, version.VersionIndex);

            version.Children.OfType<NormalItem>().Single().WidthType.ShouldBe(WidthType.Pixels);
            version.Children.OfType<NormalItem>().Single().Width.ShouldBe(123);
        }

        [Test]
        public void Unpresisted_ChildParts_AreMainteinedWhenRestoringVersions()
        {
            var master = CreateOneItem<NormalPage>(0, "master", null);
            persister.Save(master);

            var part = CreateOneItem<NormalItem>(0, "part", master);
            part.ZoneName = "TheZone";

            var part2 = CreateOneItem<NormalItem>(0, "part2", part);
            part2.ZoneName = "TheZone";

            var manager = new VersionManager(repository, persister.Repository, new StateChanger(), new N2.Configuration.EditSection());
            var version = manager.AddVersion(master, asPreviousVersion: false);

            persister.Dispose();

            master = (NormalPage)persister.Get(master.ID);
            version = manager.GetVersion(master, version.VersionIndex);

            version.Children.Single().Name.ShouldBe("part");
            version.Children.Single().Children.Single().Name.ShouldBe("part2");
        }

        [Test]
        public void Version_GetsParent_FromMasterVersion()
        {
            var parent = CreateOneItem<NormalPage>(0, "parent", null);
            persister.Save(parent);

            var master = CreateOneItem<NormalPage>(0, "master", parent);
            persister.Save(master);

            var manager = new VersionManager(repository, persister.Repository, new StateChanger(), new N2.Configuration.EditSection());
            var versionIndex = manager.AddVersion(master, asPreviousVersion: false).VersionIndex;

            persister.Dispose();
            
            var version = repository.GetVersion(master, versionIndex).Version;

            version.Parent.ShouldBe(parent);
        }

        [Test]
        public void DefaultValues_AreApplied()
        {
            var master = activator.CreateInstance(typeof(NormalPage), null, null, asProxy: true);
            persister.Save(master);

            var draft = repository.Save(master);
            repository.Repository.Dispose();

            var savedDraft = (NormalPage)repository.GetVersion(master).Version;
            savedDraft.Width.ShouldBe(2);
        }

        [Test]
        public void References_OnAutoImplementedProperties_AreMaintained()
        {
            var root = activator.CreateInstance(typeof(NormalPage), null, null, asProxy : true);
            persister.Save(root);

            var page = (NormalPage)activator.CreateInstance(typeof(NormalPage), root, null, asProxy: true);
            page.EditableLink = root;
            persister.Save(page);

            var draft = repository.Save(page);
            repository.Repository.Dispose();

            var savedDraft = (NormalPage)repository.GetVersion(page).Version;
            savedDraft.EditableLink.ShouldBe(persister.Get(root.ID));
        }

        [Test]
        public void References_OnAutoImplementedProperties_OnParts_AreMaintained()
        {
            var root = activator.CreateInstance(typeof(NormalPage), null, null, asProxy: true);
            persister.Save(root);

            var page = (NormalPage)activator.CreateInstance(typeof(NormalPage), root, null, asProxy: true);
            persister.Save(page);

            var part = (NormalItem)activator.CreateInstance(typeof(NormalItem), page, null, asProxy: true);
            part.EditableLink = root;
            persister.Save(part);

            var draft = repository.Save(page);
            repository.Repository.Dispose();

            var savedDraft = (NormalPage)repository.GetVersion(page).Version;
            ((NormalItem)savedDraft.Children[0]).EditableLink.ShouldBe(persister.Get(root.ID));
        }
    }
}
