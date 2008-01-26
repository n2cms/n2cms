using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using N2.Definitions;
using N2.Persistence.NH;
using N2.Web.UI;
using Rhino.Mocks;
using N2.Engine;
using System.Reflection;
using N2.Persistence;
using N2.Details;

namespace N2.Tests.Persistence.NH
{
	[TestFixture]
	public class ItemRepositoryTests : DatabasePreparingBase
	{
		NHRepository<int, ContentItem> repository;
		ISessionProvider sessionProvider;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			CreateDatabaseSchema();

			sessionProvider = CreateSessionProvider();
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
				ICollection<ContentItem> items = repository.FindAll(NHibernate.Expression.Expression.Gt("ID", 1));
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

		private ISessionProvider CreateSessionProvider()
		{
			MockRepository mocks = new MockRepository(); 
			using (mocks.Record())
			{
				ITypeFinder typeFinder = mocks.CreateMock<ITypeFinder>();
				Expect.On(typeFinder).Call(typeFinder.GetAssemblies()).Return(new Assembly[] { typeof(N2.Context).Assembly });
				Expect.On(typeFinder).Call(typeFinder.Find(typeof(ContentItem))).Return(new Type[] { typeof(Definitions.PersistableItem1) });
				mocks.ReplayAll();

				IDefinitionManager definitions = new DefaultDefinitionManager(new DefinitionBuilder(typeFinder, new EditableHierarchyBuilder<IEditable>(), new AttributeExplorer<EditorModifierAttribute>(), new AttributeExplorer<IDisplayable>(), new AttributeExplorer<IEditable>(), new AttributeExplorer<IEditableContainer>()), null);
				DefaultConfigurationBuilder configurationBuilder = new DefaultConfigurationBuilder(definitions);
				configurationBuilder.Properties.Add("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider");
				configurationBuilder.Properties.Add("hibernate.connection.connection_string_name", "LocalSqlServer");
				configurationBuilder.Properties.Add("hibernate.show_sql", "true");
				configurationBuilder.Properties.Add("hibernate.cache.use_second_level_cache", "false");
				configurationBuilder.Properties.Add("hibernate.connection.driver_class", "NHibernate.Driver.SqlClientDriver");
				configurationBuilder.Properties.Add("hibernate.dialect", "NHibernate.Dialect.MsSql2005Dialect");

				return new DefaultSessionProvider(configurationBuilder, new Fakes.FakeWebContextWrapper());
			}
		}
	}
}
