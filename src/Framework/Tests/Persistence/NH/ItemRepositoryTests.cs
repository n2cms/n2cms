using System.Collections.Generic;
using NUnit.Framework;
using N2.Persistence.NH;
using N2.Persistence;
using N2.Details;
using N2.Tests.Persistence.Definitions;
using System.Diagnostics;

namespace N2.Tests.Persistence.NH
{
	[TestFixture, Category("Integration")]
	public class ItemRepositoryTests : DatabasePreparingBase
	{
		NHRepository<int, ContentItem> repository;
		new ISessionProvider sessionProvider;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			CreateDatabaseSchema();

			sessionProvider = engine.Resolve<ISessionProvider>();
			repository = new NHRepository<int, ContentItem>(sessionProvider);
		}

		[Test]
		public void CanSave()
		{
			int itemID = SaveAnItem("savedItem", null);
			Assert.AreNotEqual(0, itemID);

			using (repository)
			{
				ContentItem item = repository.Get(itemID);
				Assert.AreEqual(item.ID, itemID);
				repository.Delete(item);
				repository.Flush();
			}
		}

		[Test]
		public void CanUpdate()
		{
			int itemID = SaveAnItem("savedItem", null);

			using (repository)
			{
				ContentItem item = repository.Get(itemID);
				item.Title = "updated item";
				repository.SaveOrUpdate(item);
				repository.Flush();
			}

			using (repository)
			{
				ContentItem item = repository.Get(itemID);
				Assert.AreEqual("updated item", item.Title);
				repository.Delete(item);
				repository.Flush();
			}
		}

		[Test]
		public void CanDelete()
		{
			int itemID = SaveAnItem("itemToDelete", null);

			using (repository)
			{
				ContentItem item = repository.Get(itemID);
				Assert.IsNotNull(item, "There should be a saved item.");
				repository.Delete(item);
				repository.Flush();
			}

			using (repository)
			{
				ContentItem item = repository.Get(itemID);
				Assert.IsNull(item, "Item is supposed to be deleted");
				repository.Flush();
			}
		}

		[Test]
		public void CanFindAll()
		{
			int item1ID = SaveAnItem("first", null);
			int item2ID = SaveAnItem("second", null);
			int item3ID = SaveAnItem("third", null);

			using (repository)
			{
				ICollection<ContentItem> items = repository.FindAll();
				Assert.AreEqual(3, items.Count);
				repository.Flush();
			}
		}

		[Test]
		public void CanFindGreaterThanID()
		{
			int item1ID = SaveAnItem("first", null);
			int item2ID = SaveAnItem("second", null);
			int item3ID = SaveAnItem("third", null);

			using (repository)
			{
				ICollection<ContentItem> items = repository.FindAll(NHibernate.Criterion.Expression.Gt("ID", 1));
				Assert.AreEqual(2, items.Count);
				repository.Flush();
			}
		}

		[Test]
		public void CanSaveDetail()
		{
			IRepository<int, N2.Details.ContentDetail> detailRepository = new NHRepository<int, N2.Details.ContentDetail>(sessionProvider);

			using (repository)
			{
				ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "item", null);
				item["TheString"] = "the string";
				repository.Save(item);
				repository.Flush();
			}

			using (detailRepository)
			{
				Assert.AreEqual(1, detailRepository.Count());
			}
		}

		[Test]
		public void CanDeleteDetail()
		{
			IRepository<int, ContentDetail> detailRepository = new NHRepository<int, ContentDetail>(sessionProvider);

			using (repository)
			{
				ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "item", null);
				item["TheString"] = "the string";
				repository.Save(item);
				repository.Flush();

				Assert.AreEqual(1, detailRepository.Count());

				item["TheString"] = null;
				repository.Save(item);
				repository.Flush();

				Assert.AreEqual(0, detailRepository.Count());
			}
		}

		[Test]
		public void DeleteItemCascadesDetails()
		{
			IRepository<int, ContentDetail> detailRepository = new NHRepository<int, ContentDetail>(sessionProvider);
			int itemID = 0;

			using (repository)
			{
				ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "item", null);
				item["TheString"] = "the string";
				repository.Save(item);
				repository.Flush();
				itemID = item.ID;
			}
			using(detailRepository)
			{
				Assert.AreEqual(1, detailRepository.Count());
			}
			using(repository)
			{
				ContentItem item = repository.Get(itemID);
				repository.Delete(item);
				repository.Flush();
			}
			using(detailRepository)
			{
				Assert.AreEqual(0, detailRepository.Count());
			}
		}

		[Test]
		public void ItemClasses_MayHaveNonVirtualProperties()
		{
			using (repository)
			{
				ContentItem item = CreateOneItem<Definitions.NonVirtualItem>(0, "item", null);
				repository.Save(item);
				repository.Flush();

				repository.Delete(item);
				repository.Flush();
			}
		}

		[Test]
		public void ItemClasses_MayHaveNonVirtualProperties_LazyLoading()
		{
			NonVirtualItem item;
			NonVirtualItem item2;
			NonVirtualItem item3;
			NonVirtualItem item4;
			NonVirtualItem item5;

			using (repository)
			{
				item = CreateOneItem<NonVirtualItem>(0, "item", null);
				item2 = CreateOneItem<NonVirtualItem>(0, "item2", item);
				item3 = CreateOneItem<NonVirtualItem>(0, "item3", item2);
				item4 = CreateOneItem<NonVirtualItem>(0, "item4", item3);
				item5 = CreateOneItem<NonVirtualItem>(0, "item5", item4);

				item.IntProperty = 1;
				item2.IntProperty = 2;
				item3.IntProperty = 3;
				item4.IntProperty = 4;
				item5.IntProperty = 5;

				repository.Save(item);
				repository.Flush();
			}

			using (repository)
			{
				Debug.WriteLine("A");
				
				Debug.WriteLine("one");
				item = repository.Get<NonVirtualItem>(item.ID);
				Debug.WriteLine("one.2");
				Assert.That(item.Name, Is.EqualTo("item"));
				Debug.WriteLine("one.3");
				Assert.That(item["IntProperty"], Is.EqualTo(1));

				Debug.WriteLine("two");
				Assert.That(item.Children.Count, Is.EqualTo(1));
				Debug.WriteLine("two.2");
				Assert.That(item.Children[0].Name, Is.EqualTo("item2"));
				Debug.WriteLine("two.3");
				Assert.That(item.Children[0]["IntProperty"], Is.EqualTo(2));

				Debug.WriteLine("three");
				Assert.That(item.Children[0].Children.Count, Is.EqualTo(1));
				Debug.WriteLine("three.2");
				Assert.That(item.Children[0].Children[0].Name, Is.EqualTo("item3"));
				Debug.WriteLine("three.3");
				Assert.That(item.Children[0].Children[0]["IntProperty"], Is.EqualTo(3));

				Debug.WriteLine("four");
				Assert.That(item.Children[0].Children[0].Children.Count, Is.EqualTo(1));
				Debug.WriteLine("four.2");
				Assert.That(item.Children[0].Children[0].Children[0].Name, Is.EqualTo("item4"));
				Debug.WriteLine("four.3");
				Assert.That(item.Children[0].Children[0].Children[0]["IntProperty"], Is.EqualTo(4));

				Debug.WriteLine("five");
				Assert.That(item.Children[0].Children[0].Children[0].Children.Count, Is.EqualTo(1));
				Debug.WriteLine("four.2");
				Assert.That(item.Children[0].Children[0].Children[0].Children[0].Name, Is.EqualTo("item5"));
				Debug.WriteLine("four.3");
				Assert.That(item.Children[0].Children[0].Children[0].Children[0]["IntProperty"], Is.EqualTo(5));
			}

			using (repository)
			{
				Debug.WriteLine("B");
				item4 = repository.Get<NonVirtualItem>(item4.ID);
				item = repository.Get<NonVirtualItem>(item.ID);
				item.LinkProperty = item4;
				repository.Save(item);
				repository.Flush();
			}

			using (repository)
			{
				Debug.WriteLine("C");
				item = repository.Get<NonVirtualItem>(item.ID);
				Assert.That(item.IntProperty, Is.EqualTo(1));
				Assert.That(item.LinkProperty, Is.EqualTo(item4));
				Assert.That(item.LinkProperty.Parent, Is.EqualTo(item3));
			}

			using (repository)
			{
				Debug.WriteLine("deleting");
				repository.Delete(repository.Get(item.ID));
				repository.Flush();
			}
		}

		[Test, Ignore("Any way to do this?")]
		public void OtherSideOfReferenceIsRemoved()
		{
			int itemID = 0;
			int item2ID = 0;

			using (repository)
			{
				ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "item", null);
				ContentItem item2 = CreateOneItem<Definitions.PersistableItem1>(0, "item2", null);
				item["reference"] = item2;
				repository.Save(item);
				repository.Save(item2);
				repository.Flush();
				itemID = item.ID;
				item2ID = item2.ID;
			}
			using (repository)
			{
				ContentItem item2 = repository.Get(item2ID);
				repository.Delete(item2);
				repository.Flush();
			}
			using (repository)
			{
				ContentItem item = repository.Get(itemID);
				Assert.IsNull(item["reference"], "Other side of reference has been deleted, detail should have been removed.");
				repository.Delete(item);
			}
		}

		[Test, Ignore("Any way to do this?")]
		public void CascadeDeletesWithReferences()
		{
			int itemID = 0;
			int item2ID = 0;
			int item3ID = 0;

			using (repository)
			{
				ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "item", null);
				ContentItem item2 = CreateOneItem<Definitions.PersistableItem1>(0, "item2", item);
				ContentItem item3 = CreateOneItem<Definitions.PersistableItem1>(0, "item3", item2);
				item["reference"] = item2;
				item3["reference"] = item2;
				repository.Save(item);
				repository.Save(item2);
				repository.Save(item3);
				repository.Flush();
				itemID = item.ID;
				item2ID = item2.ID;
				item3ID = item3.ID;
			}
			using (repository)
			{
				ContentItem item2 = repository.Get(item2ID);
				repository.Delete(item2);
				repository.Flush();
			}
			using (repository)
			{
				ContentItem item = repository.Get(itemID);
				Assert.IsNull(item["reference"], "other side of reference has been deleted, detail should have been removed.");

				ContentItem item3 = repository.Get(item3ID);
				Assert.IsNull(item3, "item3 should have been deleted by cascade");

				repository.Delete(item);
			}
		}

		private int SaveAnItem(string name, ContentItem parent)
		{
			using (repository)
			{
				ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, name, parent);
				repository.Save(item);
				repository.Flush();
				return item.ID;
			}
		}
	}
}
