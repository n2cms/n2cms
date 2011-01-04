using System;
using N2.Tests.Persistence.Definitions;
using NUnit.Framework;
using System.Diagnostics;
using N2.Definitions;
using N2.Persistence;
using NHibernate.Tool.hbm2ddl;
using N2.Persistence.NH.Finder;
using N2.Tests.Fakes;
using N2.Persistence.NH;

namespace N2.Tests.Persistence.NH
{
	[TestFixture]
	public class PersisterTests : PersisterTestsBase
	{
		[Test]
		public void CanSave()
		{
			ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "saveableRoot", null);
			persister.Save(item);
			Assert.AreNotEqual(0, item.ID);
		}

		[Test, Ignore]
		public void Get_Children_AreEagerlyFetched()
		{
			ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "gettableRoot", null);
			ContentItem child = CreateOneItem<Definitions.PersistableItem1>(0, "gettableChild", item);
			using (persister)
			{
				persister.Save(item);
			}

			ContentItem storedItem = persister.Get(item.ID);
			persister.Dispose();

			Assert.That(storedItem.Children.Count, Is.EqualTo(1));
		}

		[Test]
		public void SavingItemWithEmptyName_NameIsSetToNull()
		{
			ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "", null);

			persister.Save(item);

			Assert.AreEqual(item.ID.ToString(), item.Name);
		}

		[Test]
		public void CanUpdate()
		{
			ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "updatableRoot", null);

			using (persister)
			{
				item["someproperty"] = "hello";
				persister.Save(item);

				item["someproperty"] = "world";
				persister.Save(item);
			}
			using (persister)
			{
				item = persister.Get(item.ID);
				Assert.AreEqual("world", item["someproperty"]);
			}
		}

		[Test]
		public void CanDelete()
		{
			ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);

			using (persister)
			{
				persister.Save(item);
				persister.Delete(item);
			}
			using (persister)
			{
				item = persister.Get(item.ID);
				Assert.IsNull(item, "Item should have been null.");
			}
		}

		[Test]
		public void CanMove()
		{
			ContentItem root = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
			ContentItem item1 = CreateOneItem<Definitions.PersistableItem1>(0, "item1", root);
			ContentItem item2 = CreateOneItem<Definitions.PersistableItem1>(0, "item2", root);

			using (persister)
			{
				persister.Save(root);
				persister.Save(item1);
				persister.Save(item2);
			}

			using (persister)
			{
				root = persister.Get(root.ID);
				item1 = persister.Get(item1.ID);
				item2 = persister.Get(item2.ID);

				persister.Move(item2, item1);
			}

			using (persister)
			{
				root = persister.Get(root.ID);
				item1 = persister.Get(item1.ID);
				item2 = persister.Get(item2.ID);

				Assert.AreEqual(1, root.Children.Count);
				Assert.AreEqual(1, item1.Children.Count);
				Assert.AreEqual(item1, item2.Parent);
			}
		}

		[Test]
		public void CanCopy()
		{
			ContentItem root = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
			ContentItem item1 = CreateOneItem<Definitions.PersistableItem1>(0, "item1", root);
			ContentItem item2 = CreateOneItem<Definitions.PersistableItem1>(0, "item2", root);

			using (persister)
			{
				persister.Save(root);
				persister.Save(item1);
				persister.Save(item2);
			}

			using (persister)
			{
				root = persister.Get(root.ID);
				item1 = persister.Get(item1.ID);
				item2 = persister.Get(item2.ID);

				persister.Copy(item2, item1);
			}

			using (persister)
			{
				root = persister.Get(root.ID);
				item1 = persister.Get(item1.ID);
				item2 = persister.Get(item2.ID);

				Assert.AreEqual(2, root.Children.Count);
				Assert.AreEqual(1, item1.Children.Count);
				Assert.AreNotEqual(root, item1.Children[0]);
				Assert.AreNotEqual(item1, item1.Children[0]);
				Assert.AreNotEqual(item2, item1.Children[0]);
			}
		}



		[Test]
		public void CanChange_SaveAction()
		{
			ContentItem itemToSave = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);

			using (persister)
			{
				ContentItem invokedItem = null;
				EventHandler<CancellableItemEventArgs> handler = delegate(object sender, CancellableItemEventArgs e)
				{
					e.FinalAction = delegate(ContentItem item) { invokedItem = item; };
				};
				persister.ItemSaving += handler;
				persister.Save(itemToSave);
				persister.ItemSaving -= handler;

				Assert.That(invokedItem, Is.EqualTo(itemToSave));
			}
		}

		[Test]
		public void CanChange_DeleteAction()
		{
			ContentItem itemToDelete = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);

			using (persister)
			{
				ContentItem invokedItem = null;
				EventHandler<CancellableItemEventArgs> handler = delegate(object sender, CancellableItemEventArgs e)
				{
					e.FinalAction = delegate(ContentItem item) { invokedItem = item; };
				};
				persister.ItemDeleting += handler;
				persister.Delete(itemToDelete);
				persister.ItemDeleting -= handler;

				Assert.That(invokedItem, Is.EqualTo(itemToDelete));
			}
		}

		[Test]
		public void CanChange_MoveAction()
		{
			ContentItem source = CreateOneItem<Definitions.PersistableItem1>(0, "source", null);
			ContentItem destination = CreateOneItem<Definitions.PersistableItem1>(0, "destination", null);

			using (persister)
			{
				ContentItem invokedFrom = null;
				ContentItem invokedTo = null;
				EventHandler<CancellableDestinationEventArgs> handler = delegate(object sender, CancellableDestinationEventArgs e)
				{
					e.FinalAction = delegate(ContentItem from, ContentItem to)
					{
						invokedFrom = from;
						invokedTo = to;
						return null;
					};
				};
				persister.ItemMoving += handler;
				persister.Move(source, destination);
				persister.ItemMoving -= handler;

				Assert.That(invokedFrom, Is.EqualTo(source));
				Assert.That(invokedTo, Is.EqualTo(destination));
			}
		}

		[Test]
		public void CanChange_CopyAction()
		{
			ContentItem source = CreateOneItem<Definitions.PersistableItem1>(0, "source", null);
			ContentItem destination = CreateOneItem<Definitions.PersistableItem1>(0, "destination", null);

			using (persister)
			{
				ContentItem invokedFrom = null;
				ContentItem invokedTo = null;
				ContentItem copyToReturn = CreateOneItem<Definitions.PersistableItem1>(0, "copied", null);
				EventHandler<CancellableDestinationEventArgs> handler = delegate(object sender, CancellableDestinationEventArgs e)
				{
					e.FinalAction = delegate(ContentItem from, ContentItem to)
					{
						invokedFrom = from;
						invokedTo = to;
						return copyToReturn;
					};
				};
				persister.ItemCopying += handler;
				ContentItem copy = persister.Copy(source, destination);
				persister.ItemCopying -= handler;

				Assert.That(copy, Is.SameAs(copyToReturn));
				Assert.That(invokedFrom, Is.EqualTo(source));
				Assert.That(invokedTo, Is.EqualTo(destination));
			}
		}

		[Test]
		public void CanSave_Guid()
		{
			PersistableItem1 item = CreateOneItem<PersistableItem1>(0, "root", null);
			PersistableItem1 fromDB = null;

			item.GuidProperty = Guid.NewGuid();
			using (persister)
			{
				persister.Save(item);
			}

			fromDB = persister.Get<PersistableItem1>(item.ID);

			Assert.That(fromDB.GuidProperty, Is.EqualTo(item.GuidProperty));
		}

		[Test]
		public void CanSave_ReadOnlyGuid()
		{
			PersistableItem1 item = CreateOneItem<PersistableItem1>(0, "root", null);
			PersistableItem1 fromDB = null;
			string guid = item.ReadOnlyGuid;

			using (persister)
			{
				persister.Save(item);
			}

			fromDB = persister.Get<PersistableItem1>(item.ID);

			Assert.That(fromDB.ReadOnlyGuid, Is.EqualTo(guid));
		}

		[Test]
		public void CanSave_WritableGuid()
		{
			PersistableItem1 item = CreateOneItem<PersistableItem1>(0, "root", null);
			PersistableItem1 fromDB = null;

			string guid = item.WritableGuid;
			item.WritableGuid = guid;
			using (persister)
			{
				persister.Save(item);
			}

			fromDB = persister.Get<PersistableItem1>(item.ID);

			Assert.That(fromDB.WritableGuid, Is.EqualTo(guid));
		}

		[Test]
		public void Laziness()
		{
			ContentItem root = CreateOneItem<PersistableItem1>(0, "root", null);
			ContentItem root2 = CreateOneItem<PersistableItem1>(0, "root2", null);
			for (int i = 0; i < 30; i++)
			{
				PersistableItem1 item = CreateOneItem<PersistableItem1>(0, "item", root);
			}
			using (persister)
			{
				persister.Save(root);
				persister.Save(root2);
			}
			using (persister)
			{
				root = persister.Get(root.ID);
				Debug.WriteLine("Got: " + root + " with Children.Count: " + root.Children.Count);
				foreach (var child in root.Children)
				{
				}
				root2 = persister.Get(root2.ID);
				Debug.WriteLine("Got: " + root2 + " with Children.Count: " + root2.Children.Count);
				foreach (var child in root2.Children)
				{
				}
			}
		}

		[Test]
		public void Save_CausesSortOrder_ToBeUpdated()
		{
			ContentItem parent = CreateOneItem<Definitions.PersistableItem1>(0, "parent", null);
			persister.Save(parent);

			ContentItem child1 = CreateOneItem<Definitions.PersistableItem1>(0, "child1", parent);
			persister.Save(child1);
			ContentItem child2 = CreateOneItem<Definitions.PersistableItem1>(0, "child2", parent);
			persister.Save(child2);
			ContentItem child3 = CreateOneItem<Definitions.PersistableItem1>(0, "child3", parent);
			persister.Save(child3);

			Assert.That(child1.SortOrder, Is.LessThan(child2.SortOrder));
			Assert.That(child2.SortOrder, Is.LessThan(child3.SortOrder));
		}

		[Test]
		public void Save_OnParentWith_SortChildrenByUnordered_CausesSortOrder_NotToBeUpdated()
		{
			ContentItem parent = CreateOneItem<Definitions.NonVirtualItem>(0, "parent", null);
			persister.Save(parent);

			ContentItem child1 = CreateOneItem<Definitions.PersistableItem1>(0, "child1", parent);
			persister.Save(child1);
			ContentItem child2 = CreateOneItem<Definitions.PersistableItem1>(0, "child2", parent);
			persister.Save(child2);
			ContentItem child3 = CreateOneItem<Definitions.PersistableItem1>(0, "child3", parent);
			persister.Save(child3);

			Assert.That(child1.SortOrder, Is.EqualTo(0));
			Assert.That(child2.SortOrder, Is.EqualTo(0));
			Assert.That(child3.SortOrder, Is.EqualTo(0));
		}

		[Test]
		public void Save_OnParentWith_SortChildren_ByExpression_NameDesc_CausesChildrenToBeReordered()
		{
			ContentItem parent = CreateOneItem<Definitions.PersistableItem2>(0, "parent", null);
			persister.Save(parent);

			ContentItem child1 = CreateOneItem<Definitions.PersistableItem1>(0, "child1", parent);
			persister.Save(child1);
			ContentItem child2 = CreateOneItem<Definitions.PersistableItem1>(0, "child2", parent);
			persister.Save(child2);
			ContentItem child3 = CreateOneItem<Definitions.PersistableItem1>(0, "child3", parent);
			persister.Save(child3);

			Assert.That(child1.SortOrder, Is.GreaterThan(child2.SortOrder));
			Assert.That(child2.SortOrder, Is.GreaterThan(child3.SortOrder));
		}
	}
}
