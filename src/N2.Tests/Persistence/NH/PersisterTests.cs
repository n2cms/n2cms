using System;
using N2.Tests.Persistence.Definitions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace N2.Tests.Persistence.NH
{
	[TestFixture]
	public class PersisterTests : PersisterTestsBase
	{
		[Test]
		public void CanSave()
		{
			ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
			persister.Save(item);
			Assert.AreNotEqual(0, item.ID);
			persister.Delete(item);
		}

		[Test]
		public void SavingItemWithEmptyName_NameIsSetToNull()
		{
			ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "", null);
			persister.Save(item);
			Assert.AreEqual(item.ID.ToString(), item.Name);
			persister.Delete(item);
		}

		[Test]
		public void CanUpdate()
		{
			ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);

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
				persister.Delete(item);
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

				persister.Delete(root);
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

				persister.Delete(root);
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
			try
			{
				item.GuidProperty = Guid.NewGuid();
				using (persister)
				{
					persister.Save(item);
				}

				fromDB = persister.Get<PersistableItem1>(item.ID);

				Assert.That(fromDB.GuidProperty, Is.EqualTo(item.GuidProperty));
			}
			finally
			{
				persister.Delete(fromDB ?? item);
			}
		}

		[Test]
		public void CanSave_ReadOnlyGuid()
		{
			PersistableItem1 item = CreateOneItem<PersistableItem1>(0, "root", null);
			PersistableItem1 fromDB = null;
			try
			{
				string guid = item.ReadOnlyGuid;

				using (persister)
				{
					persister.Save(item);
				}

				fromDB = persister.Get<PersistableItem1>(item.ID);

				Assert.That(fromDB.ReadOnlyGuid, Is.EqualTo(guid));
			}
			finally
			{
				persister.Delete(fromDB ?? item);
			}
		}

		[Test]
		public void CanSave_WritableGuid()
		{
			PersistableItem1 item = CreateOneItem<PersistableItem1>(0, "root", null);
			PersistableItem1 fromDB = null;
			try
			{
				string guid = item.WritableGuid;
				item.WritableGuid = guid;
				using (persister)
				{
					persister.Save(item);
				}

				fromDB = persister.Get<PersistableItem1>(item.ID);

				Assert.That(fromDB.WritableGuid, Is.EqualTo(guid));
			}
			finally
			{
				persister.Delete(fromDB ?? item);

			}
		}
	}
}
