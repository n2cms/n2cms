using NUnit.Framework;

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
	}
}
