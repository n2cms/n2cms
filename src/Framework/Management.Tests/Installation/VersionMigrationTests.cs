using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Tests.Persistence;
using NUnit.Framework;
using N2.Edit.Versioning;
using N2.Persistence;
using N2.Tests;
using N2.Tests.Edit.Items;
using Shouldly;
using N2.Management.Installation;

namespace N2.Management.Tests.Installation
{
    [TestFixture]
    public class VersionMigrationTests : DatabasePreparingBase
    {
        ContentVersionRepository repository;
        IPersister persister;
        ContentActivator activator;
        private ContentItem master;
        private ContentItem version;
        private UpgradeVersionWorker worker;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            repository = TestSupport.CreateVersionRepository(ref persister, ref activator, typeof(NormalPage), typeof(NormalItem));
            master = CreateOneItem<NormalPage>(0, "root", null);
            persister.Save(master);

            version = CreateOneItem<NormalPage>(0, "root version", null);
            version.State = ContentState.Unpublished;
            version.VersionOf = master;
            version.VersionIndex = master.VersionIndex + 1;
            version.SavedBy = "test";
            persister.Save(master);

            worker = new UpgradeVersionWorker(repository, persister.Repository);
        }

        [Test]
        public void UpgradeVersion_RemovesPreviousVersion()
        {
            using (persister)
            {
                worker.UpgradeVersion(version);
            }

            var previousVersion = persister.Get(version.ID);
            previousVersion.ShouldBe(null);
        }

        [Test]
        public void UpgradeVersion_CopiesBuiltInProperties()
        {
            using (persister)
            {
                worker.UpgradeVersion(version);
            }

            var newVersion = repository.GetVersion(master, version.VersionIndex);

            //newVersion.Expired.ShouldBe(version.Expires);
            newVersion.FuturePublish.ShouldBe(version["FuturePublish"]);
            newVersion.ItemCount.ShouldBe(1);
            newVersion.Master.ID.ShouldBe(master.ID);
            newVersion.Published.ShouldBe(version.Published);
            //newVersion.PublishedBy.ShouldBe(version.SavedBy);
            newVersion.Saved.StripMilliseconds().ShouldBe(version.Updated.StripMilliseconds());
            newVersion.SavedBy.ShouldBe(version.SavedBy);
            newVersion.State.ShouldBe(version.State);
            newVersion.Title.ShouldBe(version.Title);
            newVersion.VersionIndex.ShouldBe(version.VersionIndex);
            newVersion.Version.ID.ShouldBe(0);
            newVersion.Version.Created.StripMilliseconds().ShouldBe(version.Created.StripMilliseconds());
            //newVersion.Version.Expires.ShouldBe(version.Expires);
            newVersion.Version.Name.ShouldBe(version.Name);
            newVersion.Version.Published.Value.StripMilliseconds().ShouldBe(version.Published.Value.StripMilliseconds());
            newVersion.Version.SavedBy.ShouldBe(version.SavedBy);
            newVersion.Version.SortOrder.ShouldBe(version.SortOrder);
            newVersion.Version.State.ShouldBe(version.State);
            newVersion.Version.TemplateKey.ShouldBe(version.TemplateKey);
            newVersion.Version.Title.ShouldBe(version.Title);
            newVersion.Version.TranslationKey.ShouldBe(version.TranslationKey);
            //newVersion.Version.Updated.ShouldBe(version.Updated);
            newVersion.Version.VersionIndex.ShouldBe(version.VersionIndex);
            newVersion.Version.Visible.ShouldBe(version.Visible);
            newVersion.Version.ZoneName.ShouldBe(version.ZoneName);
        }

        [Test]
        public void UpgradeVersion_CopiesDetails()
        {
            using (persister)
            {
                version["Hello"] = "World";
                persister.Save(version);
                worker.UpgradeVersion(version);
            }

            var newVersion = repository.GetVersion(master, version.VersionIndex);
            newVersion.Version["Hello"].ShouldBe("World");
        }

        [Test]
        public void UpgradeVersion_CopiesDetailCollections()
        {
            using (persister)
            {
                version.GetDetailCollection("Hello").Add("World");
                persister.Save(version);
                worker.UpgradeVersion(version);
            }

            var newVersion = repository.GetVersion(master, version.VersionIndex);
            version.GetDetailCollection("Hello", createWhenEmpty: false)[0].ShouldBe("World");
        }

        [Test]
        public void UpgradeVersion_AddParts_OfPublishedVersion()
        {
            using (persister)
            {
                var part = CreateOneItem<NormalItem>(0, "part", master);
                part.ZoneName = "TheZone";
                persister.Save(part);

                worker.UpgradeVersion(version);
            }

            var newVersion = repository.GetVersion(master, version.VersionIndex);
            newVersion.Version.Children.Single().ZoneName.ShouldBe("TheZone");
        }
    }
}
