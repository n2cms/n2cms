using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Web.Parts;
using N2.Edit;
using N2.Configuration;
using N2.Edit.Versioning;
using N2.Persistence;
using N2.Web;
using N2.Persistence.Sources;
using N2.Security;
using System.Collections.Specialized;
using Shouldly;
using N2.Details;
using N2.Integrity;
using N2.Tests.Fakes;

namespace N2.Tests.Web.Parts
{
    [TestFixture]
    public class ItemMoverTests : ItemPersistenceMockingBase
    {
        private NameValueCollection request;
        private Items.PageItem root;
        private ContentVersionRepository versionRepository;
        private ItemMover mover;
        private VersionManager versions;
        ContentActivator activator;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var types = new[] { typeof(Items.PageItem), typeof(Items.DataItem) };
            versionRepository = TestSupport.CreateVersionRepository(ref persister, ref activator, types);
            versions = TestSupport.SetupVersionManager(persister, versionRepository);
            mover = new ItemMover(
                persister,
                new Navigator(persister, TestSupport.SetupHost(), new VirtualNodeFactory(), TestSupport.SetupContentSource()),
                new FakeIntegrityManager(),
                versions,
                versionRepository);
            request = new NameValueCollection();

            root = CreateOneItem<Items.PageItem>(0, "root", null);
        }

        [Test]
        public void MovingPublishedItem_BetweenZones_CreatesDraft_ContainingChanges()
        {
            var part = CreateOneItem<Items.DataItem>(0, "part", root);
            part.ZoneName = "ZoneOne";

            request[PathData.ItemQueryKey] = part.Path;
            request["below"] = root.Path;
            request["zone"] = "ZoneTwo";

            var response = mover.HandleRequest(request);

            var draft = versionRepository.GetVersions(root).Single();
			versionRepository.DeserializeVersion(draft).Children.Single().ZoneName.ShouldBe("ZoneTwo");
        }

        [Test]
        public void MovingPublishedItem_BeforePublishedItem_CreatesDraft_ContainingChanges()
        {
            var part = CreateOneItem<Items.DataItem>(0, "part", root);
            part.ZoneName = "ZoneOne";

            var part2 = CreateOneItem<Items.DataItem>(0, "part2", root);
            part2.ZoneName = "ZoneOne";

            request[PathData.ItemQueryKey] = part2.Path;
            request["below"] = root.Path;
            request["before"] = part.Path;
            request["zone"] = "ZoneOne";

            var response = mover.HandleRequest(request);

            var draft = versionRepository.GetVersions(root).Single();
            versionRepository.DeserializeVersion(draft).Children[0].Name.ShouldBe("part2");
            versionRepository.DeserializeVersion(draft).Children[1].Name.ShouldBe("part");
        }

        [Test]
        public void MovingParts_OnDraft_UsesSameVersion()
        {
            var part = CreateOneItem<Items.DataItem>(0, "part", root);
            part.ZoneName = "ZoneOne";

            var version = versions.AddVersion(root, asPreviousVersion: false);
            part = version.Children[0] as Items.DataItem;
            versionRepository.GetVersions(root).Count().ShouldBe(1);

            request[PathData.ItemQueryKey] = part.Path;
            request[PathData.VersionKeyQueryKey] = part.GetVersionKey();
            request[PathData.VersionIndexQueryKey] = part.VersionIndex.ToString();
            request["below"] = root.Path;
            request["zone"] = "ZoneTwo";

            var response = mover.HandleRequest(request);

            var draft = versionRepository.GetVersions(root).Single();
			versionRepository.DeserializeVersion(draft).Children.Single().ZoneName.ShouldBe("ZoneTwo");
        }

        [Test]
        public void MovingPart_BeforeOther_OnDraft_UsesSameVersion()
        {
            var part = CreateOneItem<Items.DataItem>(0, "part", root);
            part.ZoneName = "ZoneOne";

            var part2 = CreateOneItem<Items.DataItem>(0, "part2", root);
            part2.ZoneName = "ZoneOne";

            var version = versions.AddVersion(root, asPreviousVersion: false);
            part = version.Children[0] as Items.DataItem;
            part2 = version.Children[1] as Items.DataItem;
            versionRepository.GetVersions(root).Count().ShouldBe(1);

            request[PathData.ItemQueryKey] = part2.Path;
            request[PathData.VersionKeyQueryKey] = part2.GetVersionKey();
            request[PathData.VersionIndexQueryKey] = part2.VersionIndex.ToString();
            request["before"] = part.Path;
            request["zone"] = "ZoneTwo";

            var response = mover.HandleRequest(request);

            var draft = versionRepository.GetVersions(root).Single();
            versionRepository.DeserializeVersion(draft).Children[0].Name.ShouldBe("part2");
            versionRepository.DeserializeVersion(draft).Children[1].Name.ShouldBe("part");
        }

        [Test]
        public void MovingPart_InDraft()
        {
            var version = versions.AddVersion(root, asPreviousVersion: false);
            versionRepository.GetVersions(root).Count().ShouldBe(1);
            var part = CreateOneItem<Items.DataItem>(0, "part", version);
            part.ID = 0;
            part.ZoneName = "ZoneOne";
            part.SetVersionKey("one");
            versionRepository.Save(version);

            request[PathData.ItemQueryKey] = root.Path;
            request["below"] = root.Path;
            request[PathData.VersionKeyQueryKey] = part.GetVersionKey();
            request[PathData.VersionIndexQueryKey] = version.VersionIndex.ToString();
            request["zone"] = "ZoneTwo";

            var response = mover.HandleRequest(request);

            var draft = versionRepository.GetVersions(root).Single();
            versionRepository.DeserializeVersion(draft).Children.Single().ZoneName.ShouldBe("ZoneTwo");
        }

        [Test]
        public void MovingPart_BeforeOtherPart_ThatOnlyExists_InDraft()
        {
            var version = versions.AddVersion(root, asPreviousVersion: false);
            versionRepository.GetVersions(root).Count().ShouldBe(1);

            var part = CreateOneItem<Items.DataItem>(0, "part", version);
            part.ID = 0;
            part.ZoneName = "ZoneOne";
            part.SetVersionKey("one");
            var part2 = CreateOneItem<Items.DataItem>(0, "part2", version);
            part2.ID = 0;
            part2.ZoneName = "ZoneOne";
            part2.SetVersionKey("two");
            versionRepository.Save(version);

            request[PathData.ItemQueryKey] = root.Path;
            request[PathData.VersionKeyQueryKey] = part2.GetVersionKey();
            request[PathData.VersionIndexQueryKey] = version.VersionIndex.ToString();
            request["before"] = "";
            request["beforeVersionKey"] = part.GetVersionKey();
            request["zone"] = "ZoneOne";

            var response = mover.HandleRequest(request);

            var draft = versionRepository.GetVersions(root).Single();
            versionRepository.DeserializeVersion(draft).Children[0].Name.ShouldBe("part2");
            versionRepository.DeserializeVersion(draft).Children[1].Name.ShouldBe("part");
        }

        [Test]
        public void MovingPublishedItem_BetweenZones_RedirectsToDraft()
        {
            var part = CreateOneItem<Items.DataItem>(0, "part", root);
            part.ZoneName = "ZoneOne";

            request[PathData.ItemQueryKey] = part.Path;
            request["below"] = root.Path;
            request["zone"] = "ZoneTwo";

            var response = mover.HandleRequest(request);

            response["redirect"].ShouldStartWith("/root?n2versionIndex=1&edit=drag");
        }

    }
}
