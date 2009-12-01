using System.Threading;
using N2.Edit;
using N2.Persistence;
using N2.Persistence.NH;
using N2.Tests.Persistence.Definitions;
using NUnit.Framework;
using N2.Engine;

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
			tracker = new TrailTracker(persister);
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
			PersistableItem1 item = CreateOneItem<PersistableItem1>(0, "root", null);

			persister.Save(item);

			Assert.That(item.AncestralTrail, Is.EqualTo("/"));
		}

		[Test]
		public void AncestralTrail_ContainsParentTrail_OnChildPage()
		{
			PersistableItem1 root = CreateOneItem<PersistableItem1>(0, "root", null);
			PersistableItem1 item = CreateOneItem<PersistableItem1>(0, "item", root);

			persister.Save(root);
			persister.Save(item);

			Assert.That(item.AncestralTrail, Is.EqualTo("/" + root.ID + "/"));
		}

		[Test]
		public void AncestralTrail_ContainsParentTrail_OnChildPage_WhenSavedByCascade()
		{
			PersistableItem1 root = CreateOneItem<PersistableItem1>(0, "root", null);
			PersistableItem1 item = CreateOneItem<PersistableItem1>(0, "item", root);

			persister.Save(root);

			Assert.That(item.AncestralTrail, Is.EqualTo("/" + root.ID + "/"));
		}

		[Test]
		public void AncestralTrail_IsUpdated_WhenItem_IsCopied()
		{
			PersistableItem1 root = CreateOneItem<PersistableItem1>(0, "root", null);
			PersistableItem1 one = CreateOneItem<PersistableItem1>(0, "one", root);
			PersistableItem1 two = CreateOneItem<PersistableItem1>(0, "two", root);
			persister.Save(root);

			ContentItem copiedItem = persister.Copy(two, one);

			Assert.That(copiedItem.AncestralTrail, Is.EqualTo("/" + root.ID + "/" + one.ID + "/"));
		}

		[Test]
		public void AncestralTrail_IsUpdated_WhenItem_IsMoved()
		{
			PersistableItem1 root = CreateOneItem<PersistableItem1>(0, "root", null);
			PersistableItem1 one = CreateOneItem<PersistableItem1>(0, "one", root);
			PersistableItem1 two = CreateOneItem<PersistableItem1>(0, "two", root);
			persister.Save(root);

			persister.Move(two, one);

			Assert.That(two.AncestralTrail, Is.EqualTo("/" + root.ID + "/" + one.ID + "/"));
		}

		[Test]
		public void AncestralTrail_IsUpdated_OnChildren_OfCopiedItems()
		{
			//	root
			//		one
			//		two
			//			three
			PersistableItem1 root = CreateOneItem<PersistableItem1>(0, "root", null);
			PersistableItem1 one = CreateOneItem<PersistableItem1>(0, "one", root);
			PersistableItem1 two = CreateOneItem<PersistableItem1>(0, "two", root);
			PersistableItem1 three = CreateOneItem<PersistableItem1>(0, "three", two);
			persister.Save(root);

			//	root
			//		one
			//			two (copied)
			//				three (copied child)
			//		two
			//			three
			var copiedItem = persister.Copy(two, one);

			Assert.That(copiedItem.Children[0].AncestralTrail, Is.EqualTo("/" + root.ID + "/" + one.ID + "/" + copiedItem.ID + "/"));
		}

		[Test]
		public void AncestralTrail_IsUpdated_OnChildren_OfMovedItems()
		{
			//	root
			//		one
			//		two
			//			three
			PersistableItem1 root = CreateOneItem<PersistableItem1>(0, "root", null);
			PersistableItem1 one = CreateOneItem<PersistableItem1>(0, "one", root);
			PersistableItem1 two = CreateOneItem<PersistableItem1>(0, "two", root);
			PersistableItem1 three = CreateOneItem<PersistableItem1>(0, "three", two);
			persister.Save(root);

			//	root
			//		one
			//			two (moved)
			//				three (moved child)
			persister.Move(two, one);

			Assert.That(three.AncestralTrail, Is.EqualTo("/" + root.ID + "/" + one.ID + "/" + two.ID + "/"));
		}

		[Test]
		public void AncestralTrail_IsUpdated_WhenUsing_VersioningManager()
		{
			PersistableItem1 root = CreateOneItem<PersistableItem1>(0, "root", null);
			PersistableItem1 one = CreateOneItem<PersistableItem1>(0, "one", root);
			persister.Save(root);

			N2.Persistence.VersionManager vm = new VersionManager(persister.Repository, finder, new N2.Workflow.StateChanger());
			var version = vm.SaveVersion(one);
			
			one.Name += "2";
			persister.Save(one);

			Assert.That(version.AncestralTrail, Is.EqualTo("/"));
			Assert.That(one.AncestralTrail, Is.EqualTo("/" + root.ID + "/"));
		}
	}
}
