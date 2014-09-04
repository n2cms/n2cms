using System.Threading;
using N2.Engine;
using N2.Persistence;
using N2.Persistence.NH;
using N2.Tests.Persistence.Definitions;
using NUnit.Framework;
using N2.Tests.Fakes;
using N2.Web;
using N2.Edit.Versioning;
using N2.Edit;

namespace N2.Tests.Persistence.NH
{
    [TestFixture]
    public class TrailTrackerTests : PersisterTestsBase
    {
        TrailTracker tracker;
        
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            AsyncWorker worker = new AsyncWorker();
            worker.QueueUserWorkItem = delegate(WaitCallback function)
            {
                function(null);
                return true;
            };
            tracker = new TrailTracker(persister, new TreeSorter(persister, null, null));
            tracker.Start();
        }

        [TearDown]
        public override void TearDown()
        {
            tracker.Stop();
            base.TearDown();
        }

        [Test]
        public void AncestralTrail_IsSlash_OnRootPage()
        {
            PersistableItem item = CreateOneItem<PersistableItem>(0, "root", null);

            persister.Save(item);

            Assert.That(item.AncestralTrail, Is.EqualTo("/"));
        }

        [Test]
        public void AncestralTrail_ContainsParentTrail_OnChildPage()
        {
            PersistableItem root = CreateOneItem<PersistableItem>(0, "root", null);
            PersistableItem item = CreateOneItem<PersistableItem>(0, "item", root);

            persister.Save(root);
            persister.Save(item);

            Assert.That(item.AncestralTrail, Is.EqualTo("/" + root.ID + "/"));
        }

        [Test]
        public void AncestralTrail_ContainsParentTrail_OnChildPage_WhenSavedByCascade()
        {
            PersistableItem root = CreateOneItem<PersistableItem>(0, "root", null);
            PersistableItem item = CreateOneItem<PersistableItem>(0, "item", root);

            persister.Repository.SaveOrUpdate(root, item);

            Assert.That(item.AncestralTrail, Is.EqualTo("/" + root.ID + "/"));
        }

        [Test]
        public void AncestralTrail_IsUpdated_WhenItem_IsCopied()
        {
            PersistableItem root = CreateOneItem<PersistableItem>(0, "root", null);
            PersistableItem one = CreateOneItem<PersistableItem>(0, "one", root);
            PersistableItem two = CreateOneItem<PersistableItem>(0, "two", root);
            persister.Repository.SaveOrUpdate(root, one, two);

            ContentItem copiedItem = persister.Copy(two, one);

            Assert.That(copiedItem.AncestralTrail, Is.EqualTo("/" + root.ID + "/" + one.ID + "/"));
        }

        [Test]
        public void AncestralTrail_IsUpdated_WhenItem_IsMoved()
        {
            PersistableItem root = CreateOneItem<PersistableItem>(0, "root", null);
            PersistableItem one = CreateOneItem<PersistableItem>(0, "one", root);
            PersistableItem two = CreateOneItem<PersistableItem>(0, "two", root);
            persister.Repository.SaveOrUpdate(root, one, two);

            persister.Move(two, one);

            Assert.That(two.AncestralTrail, Is.EqualTo("/" + root.ID + "/" + one.ID + "/"));
        }

        [Test]
        public void AncestralTrail_IsUpdated_OnChildren_OfCopiedItems()
        {
            //  root
            //      one
            //      two
            //          three
            PersistableItem root = CreateOneItem<PersistableItem>(0, "root", null);
            PersistableItem one = CreateOneItem<PersistableItem>(0, "one", root);
            PersistableItem two = CreateOneItem<PersistableItem>(0, "two", root);
            PersistableItem three = CreateOneItem<PersistableItem>(0, "three", two);
            persister.Repository.SaveOrUpdate(root, one, two, three);

            //  root
            //      one
            //          two (copied)
            //              three (copied child)
            //      two
            //          three
            var copiedItem = persister.Copy(two, one);

            Assert.That(copiedItem.Children[0].AncestralTrail, Is.EqualTo("/" + root.ID + "/" + one.ID + "/" + copiedItem.ID + "/"));
        }

        [Test]
        public void AncestralTrail_IsUpdated_OnChildren_OfMovedItems()
        {
            //  root
            //      one
            //      two
            //          three
            PersistableItem root = CreateOneItem<PersistableItem>(0, "root", null);
            PersistableItem one = CreateOneItem<PersistableItem>(0, "one", root);
            PersistableItem two = CreateOneItem<PersistableItem>(0, "two", root);
            PersistableItem three = CreateOneItem<PersistableItem>(0, "three", two);
            persister.Repository.SaveOrUpdate(root, one, two, three);

            //  root
            //      one
            //          two (moved)
            //              three (moved child)
            persister.Move(two, one);

            Assert.That(three.AncestralTrail, Is.EqualTo("/" + root.ID + "/" + one.ID + "/" + two.ID + "/"));
        }

        [Test]
        public void AncestralTrail_IsUpdated_WhenUsing_VersioningManager()
        {
            PersistableItem root = CreateOneItem<PersistableItem>(0, "root", null);
            PersistableItem one = CreateOneItem<PersistableItem>(0, "one", root);
            persister.Repository.SaveOrUpdate(root, one);

            VersionManager vm = new VersionManager(TestSupport.CreateVersionRepository(typeof(PersistableItem)), persister.Repository, new N2.Edit.Workflow.StateChanger(), new N2.Configuration.EditSection());
            var version = vm.AddVersion(one);
            
            one.Name += "2";
            persister.Save(one);

            Assert.That(version.AncestralTrail, Is.EqualTo("/"));
            Assert.That(one.AncestralTrail, Is.EqualTo("/" + root.ID + "/"));
        }
    }
}
