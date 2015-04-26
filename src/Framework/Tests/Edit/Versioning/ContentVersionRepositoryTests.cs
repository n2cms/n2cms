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

namespace N2.Tests.Edit.Versioning
{
    [TestFixture]
    public class ContentVersionRepositoryTests : ItemTestsBase
    {
        ContentVersionRepository repository;
        IPersister persister;
        ContentActivator activator;
        private DraftRepository drafts;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            persister = null;
            activator = null;
            repository = TestSupport.CreateVersionRepository(ref persister, ref activator, typeof(Items.NormalPage), typeof(Items.NormalItem));
            drafts = new DraftRepository(repository, new FakeCacheWrapper());
        }

        [Test]
        public void MasterVersion_CanBeSavedAsVersion_AndRetrieved()
        {
            var master = CreateOneItem<Items.NormalPage>(0, "pageX", null);
            persister.Save(master);

            var draft = repository.Save(master);
            repository.Repository.Dispose();

            var savedDraft = repository.GetVersion(master);
            savedDraft.Published.ShouldBe(master.Published, TimeSpan.FromSeconds(1));
            //savedDraft.PublishedBy.ShouldBe(master.SavedBy);
            savedDraft.Saved.ShouldBe(N2.Utility.CurrentTime(), TimeSpan.FromSeconds(1));
            savedDraft.SavedBy.ShouldBe(draft.SavedBy);
            savedDraft.State.ShouldBe(master.State);
            savedDraft.VersionIndex.ShouldBe(master.VersionIndex);
            savedDraft.VersionDataXml.ShouldContain("pageX");
        }

        [Test]
        public void Version_CanBeSavedAsVersioin_AndRetrieved()
        {
            var page = CreateOneItem<Items.NormalPage>(0, "page", null);
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
            var page = CreateOneItem<Items.NormalPage>(0, "page", null);
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
            var page = CreateOneItem<Items.NormalPage>(0, "page", null);
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
            var page = CreateOneItem<Items.NormalPage>(0, "page", null);
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
            var page = CreateOneItem<Items.NormalPage>(0, "page", null);
            persister.Save(page);

            var versionItem = page.Clone(true);
            versionItem.VersionIndex = page.VersionIndex + 1;
            versionItem.State = ContentState.Draft;
            versionItem.VersionOf = page;
            
            var version = repository.Save(versionItem);

            repository.Repository.Dispose();
            var savedVersion = repository.GetVersion(page, versionItem.VersionIndex);

            savedVersion.VersionIndex.ShouldBe(versionItem.VersionIndex);
            repository.DeserializeVersion(savedVersion).VersionIndex.ShouldBe(versionItem.VersionIndex);
        }

        [Test]
        public void Parts_AreSerialized()
        {
            var page = CreateOneItem<Items.NormalPage>(0, "page", null);
            persister.Save(page);
            var part = CreateOneItem<Items.NormalItem>(0, "part", page);
            part.ZoneName = "TheZone";
            persister.Save(part);

            var version = page.CloneForVersioningRecursive(new StateChanger());
            version.VersionIndex = page.VersionIndex + 1;
            version.VersionOf = page;

            repository.Save(version);

            repository.Repository.Dispose();
            var savedVersion = repository.GetVersion(page, version.VersionIndex);

			var deserializedPage = repository.DeserializeVersion(savedVersion);
            var deserializedPart = deserializedPage.Children.Single();

            deserializedPart.Title.ShouldBe("part");
            deserializedPart.VersionOf.ID.ShouldBe(part.ID);
            deserializedPart.ZoneName.ShouldBe(part.ZoneName);
        }

        [Test]
        public void Details_AreSerialized()
        {
            var page = CreateOneItem<Items.NormalPage>(0, "page", null);
            page["Hello"] = "world";
            page.GetDetailCollection("Stuffs", true).Add("Hello");
            persister.Save(page);

            var version = page.Clone(true);
            version.VersionIndex = page.VersionIndex + 1;
            version.VersionOf = page;

            repository.Save(version);

            repository.Repository.Dispose();
            var savedVersion = repository.GetVersion(page, version.VersionIndex);

			var deserializedPage = repository.DeserializeVersion(savedVersion);
            deserializedPage["Hello"].ShouldBe("world");
            deserializedPage.GetDetailCollection("Stuffs", false)[0].ShouldBe("Hello");
        }

        [Test]
        public void Details_OnParts_AreSerialized()
        {
            var page = CreateOneItem<Items.NormalPage>(0, "page", null);
            persister.Save(page);
            var part = CreateOneItem<Items.NormalItem>(0, "part", page);
            part["Hello"] = "world";
            part.GetDetailCollection("Stuffs", true).Add("Hello");
            part.ZoneName = "TheZone";
            persister.Save(part);

            var version = page.CloneForVersioningRecursive(new StateChanger());
            version.VersionIndex = page.VersionIndex + 1;

            repository.Save(version);

            repository.Repository.Dispose();
            var savedVersion = repository.GetVersion(page, version.VersionIndex);

            var deserializedPage = repository.DeserializeVersion(savedVersion);
            var deserializedPart = deserializedPage.Children.Single();
            deserializedPart["Hello"].ShouldBe("world");
            deserializedPart.GetDetailCollection("Stuffs", false)[0].ShouldBe("Hello");
        }

        [Test]
        public void NotSavedAsPrevious_IsFoundAsDraft()
        {
            var master = CreateOneItem<Items.NormalPage>(0, "pageX", null);
            persister.Save(master);

            var manager = new VersionManager(repository, persister.Repository, new StateChanger(), new N2.Configuration.EditSection());
            manager.AddVersion(master, asPreviousVersion: false);

            drafts.FindDrafts().Single().Master.ID.ShouldBe(master.ID);
        }

        [Test]
        public void SavedAsPrevious_IsNotFoundAsDraft()
        {
            var master = CreateOneItem<Items.NormalPage>(0, "pageX", null);
            persister.Save(master);

            var manager = new VersionManager(repository, persister.Repository, new StateChanger(), new N2.Configuration.EditSection());
            manager.AddVersion(master, asPreviousVersion: true);

            drafts.FindDrafts().Any().ShouldBe(false);
        }

        [Test]
        public void DraftsCanBeFound_ForSingleItem()
        {
            var master = CreateOneItem<Items.NormalPage>(0, "pageX", null);
            persister.Save(master);

            var manager = new VersionManager(repository, persister.Repository, new StateChanger(), new N2.Configuration.EditSection());
            manager.AddVersion(master, asPreviousVersion: false);

            drafts.FindDrafts().Single().Master.ID.ShouldBe(master.ID);
        }

        [Test]
        public void AutoImplementedProperties_AreMaintained()
        {
            var master = CreateOneItem<Items.NormalPage>(0, "master", null);
            master.WidthType = Items.WidthType.Pixels;
            master.Width = 123;
            persister.Save(master);

            var manager = new VersionManager(repository, persister.Repository, new StateChanger(), new N2.Configuration.EditSection());
            var version = (Items.NormalPage)manager.AddVersion(master, asPreviousVersion: false);

            persister.Dispose();

            master = (Items.NormalPage)persister.Get(master.ID);
            version = (Items.NormalPage)manager.GetVersion(master, version.VersionIndex);

            version.WidthType.ShouldBe(Items.WidthType.Pixels);
            version.Width.ShouldBe(123);
        }

        [Test]
        public void AutoImplementedProperties_AreMaintained_OnPersistedItems()
        {
            var master = CreateOneItem<Items.NormalPage>(0, "master", null);
            master.WidthType = Items.WidthType.Pixels;
            master.Width = 123;
			persister.Save(master);

            persister.Dispose();
            master = (Items.NormalPage)persister.Get(master.ID);

            var manager = new VersionManager(repository, persister.Repository, new StateChanger(), new N2.Configuration.EditSection());
            var version = (Items.NormalPage)manager.AddVersion(master, asPreviousVersion: false);

            persister.Dispose();

            master = (Items.NormalPage)persister.Get(master.ID);
            version = (Items.NormalPage)manager.GetVersion(master, version.VersionIndex);

            version.WidthType.ShouldBe(Items.WidthType.Pixels);
            version.Width.ShouldBe(123);
        }

        [Test]
        public void AutoImplementedProperties_AreMaintained_OnParts()
        {
            var master = CreateOneItem<Items.NormalPage>(0, "master", null);
            persister.Save(master);

            var part = CreateOneItem<Items.NormalItem>(0, "part", master);
            part.WidthType = Items.WidthType.Pixels;
            part.Width = 123;
            part.ZoneName = "TheZone";
            persister.Save(part);

            var manager = new VersionManager(repository, persister.Repository, new StateChanger(), new N2.Configuration.EditSection());
            var version = (Items.NormalPage)manager.AddVersion(master, asPreviousVersion: false);

            persister.Dispose();

            master = (Items.NormalPage)persister.Get(master.ID);
            version = (Items.NormalPage)manager.GetVersion(master, version.VersionIndex);

            version.Children.OfType<Items.NormalItem>().Single().WidthType.ShouldBe(Items.WidthType.Pixels);
            version.Children.OfType<Items.NormalItem>().Single().Width.ShouldBe(123);
        }

        [Test]
        public void AutoImplementedProperties_AreMaintained_OnPeristedParts()
        {
            var master = CreateOneItem<Items.NormalPage>(0, "master", null);
            persister.Save(master);

            var part = CreateOneItem<Items.NormalItem>(0, "part", master);
            part.WidthType = Items.WidthType.Pixels;
            part.Width = 123;
            part.ZoneName = "TheZone";
            persister.Save(part);

            persister.Dispose();
            master = (Items.NormalPage)persister.Get(master.ID);

            var manager = new VersionManager(repository, persister.Repository, new StateChanger(), new N2.Configuration.EditSection());
            var version = (Items.NormalPage)manager.AddVersion(master, asPreviousVersion: false);

            persister.Dispose();

            master = (Items.NormalPage)persister.Get(master.ID);
            version = (Items.NormalPage)manager.GetVersion(master, version.VersionIndex);

            version.Children.OfType<Items.NormalItem>().Single().WidthType.ShouldBe(Items.WidthType.Pixels);
            version.Children.OfType<Items.NormalItem>().Single().Width.ShouldBe(123);
        }

        [Test]
        public void Unpresisted_ChildParts_AreMainteinedWhenRestoringVersions()
        {
            var master = CreateOneItem<Items.NormalPage>(0, "master", null);
            persister.Save(master);

            var part = CreateOneItem<Items.NormalItem>(0, "part", master);
            part.ZoneName = "TheZone";

            var part2 = CreateOneItem<Items.NormalItem>(0, "part2", part);
            part2.ZoneName = "TheZone";

            var manager = new VersionManager(repository, persister.Repository, new StateChanger(), new N2.Configuration.EditSection());
            var version = manager.AddVersion(master, asPreviousVersion: false);

            persister.Dispose();

            master = (Items.NormalPage)persister.Get(master.ID);
            version = manager.GetVersion(master, version.VersionIndex);

            version.Children.Single().Name.ShouldBe("part");
            version.Children.Single().Children.Single().Name.ShouldBe("part2");
        }

        [Test]
        public void Version_GetsParent_FromMasterVersion()
        {
            var parent = CreateOneItem<Items.NormalPage>(0, "parent", null);
            persister.Save(parent);

            var master = CreateOneItem<Items.NormalPage>(0, "master", parent);
            persister.Save(master);

            var manager = new VersionManager(repository, persister.Repository, new StateChanger(), new N2.Configuration.EditSection());
            var versionIndex = manager.AddVersion(master, asPreviousVersion: false).VersionIndex;

            persister.Dispose();
            
            var version = repository.DeserializeVersion(repository.GetVersion(master, versionIndex));

            version.Parent.ShouldBe(parent);
        }

        [Test]
        public void DefaultValues_AreApplied()
        {
            var master = activator.CreateInstance(typeof(Items.NormalPage), null, null, asProxy: true);
            persister.Save(master);

            var draft = repository.Save(master);
            repository.Repository.Dispose();

            var savedDraft = (Items.NormalPage)repository.DeserializeVersion(repository.GetVersion(master));
            savedDraft.Width.ShouldBe(2);
        }

        [Test]
        public void References_OnAutoImplementedProperties_AreMaintained()
        {
            var root = activator.CreateInstance(typeof(Items.NormalPage), null, null, asProxy : true);
            persister.Save(root);

            var page = (Items.NormalPage)activator.CreateInstance(typeof(Items.NormalPage), root, null, asProxy: true);
            page.EditableLink = root;
            persister.Save(page);

            var draft = repository.Save(page);
            repository.Repository.Dispose();

			var savedDraft = (Items.NormalPage)repository.DeserializeVersion(repository.GetVersion(page));
            savedDraft.EditableLink.ShouldBe(persister.Get(root.ID));
        }

        [Test]
        public void References_OnAutoImplementedProperties_OnParts_AreMaintained()
        {
            var root = activator.CreateInstance(typeof(Items.NormalPage), null, null, asProxy: true);
            persister.Save(root);

            var page = (Items.NormalPage)activator.CreateInstance(typeof(Items.NormalPage), root, null, asProxy: true);
            persister.Save(page);

            var part = (Items.NormalItem)activator.CreateInstance(typeof(Items.NormalItem), page, null, asProxy: true);
            part.EditableLink = root;
            persister.Save(part);

            var draft = repository.Save(page);
            repository.Repository.Dispose();

			var savedDraft = (Items.NormalPage)repository.DeserializeVersion(repository.GetVersion(page));
            ((Items.NormalItem)savedDraft.Children[0]).EditableLink.ShouldBe(persister.Get(root.ID));
        }

		[Test]
		public void Part_PresentOnDraft_IsConsideredToHaveADraft()
		{
			var pastTime = new TimeCapsule(DateTime.Now.AddSeconds(-10));

			var page = CreateOneItem<Items.NormalPage>(0, "page", null);
			persister.Save(page);

			var part = CreateOneItem<Items.NormalItem>(0, "part", page);
			part.ZoneName = "TheZone";
			persister.Save(part);

			pastTime.Dispose();

			var version = page.Clone(true);
			version.State = ContentState.Draft;
			version.VersionOf = page;
			version.VersionIndex++;

			var draft = repository.Save(version);

			drafts.HasDraft(part).ShouldBe(true);
		}

		[Test]
		public void Part_PresentOnDraft_HaveInfo()
		{
			var pastTime = new TimeCapsule(DateTime.Now.AddSeconds(-10));

			var page = CreateOneItem<Items.NormalPage>(0, "page", null);
			persister.Save(page);

			var part = CreateOneItem<Items.NormalItem>(0, "part", page);
			part.ZoneName = "TheZone";
			persister.Save(part);

			pastTime.Dispose();

			var version = page.Clone(true);
			version.State = ContentState.Draft;
			version.VersionOf = page;
			version.VersionIndex++;

			var draft = repository.Save(version);

			var info = drafts.GetDraftInfo(part);
			info.ItemID.ShouldBe(part.ID);
			info.VersionIndex.ShouldBe(version.Children[0].VersionIndex);
		}
    }
}
