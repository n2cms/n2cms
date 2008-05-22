using System;
using System.Reflection;
using NUnit.Framework;
using N2.Definitions;
using N2.Details;
using N2.Engine;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Persistence.NH;
using N2.Persistence.NH.Finder;
using N2.Web.UI;
using Rhino.Mocks;

namespace N2.Tests.Persistence.NH
{
	[TestFixture]
	public class VersionManagerTests : ItemTestsBase
	{
		DefaultPersister persister;
		DefaultSessionProvider sessionProvider;
		IItemFinder finder;
		VersionManager versioner;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			mocks = new MockRepository();

			ITypeFinder typeFinder = mocks.CreateMock<ITypeFinder>();
			Expect.On(typeFinder)
				.Call(typeFinder.GetAssemblies())
				.Return(new Assembly[] { typeof(Definitions.PersistableItem1).Assembly })
				.Repeat.Any();
			Expect.On(typeFinder)
				.Call(typeFinder.Find(typeof(ContentItem)))
				.Return(new Type[] { typeof(Definitions.PersistableItem1) })
				.Repeat.AtLeastOnce();

			mocks.ReplayAll();

			IDefinitionManager definitions = new DefaultDefinitionManager(new DefinitionBuilder(typeFinder, new EditableHierarchyBuilder<IEditable>(), new AttributeExplorer<EditorModifierAttribute>(), new AttributeExplorer<IDisplayable>(), new AttributeExplorer<IEditable>(), new AttributeExplorer<IEditableContainer>()), null);
			DefaultConfigurationBuilder configurationBuilder = new DefaultConfigurationBuilder(definitions);
			SetConfigurationProperties(configurationBuilder);

			sessionProvider = new DefaultSessionProvider(configurationBuilder, new Fakes.FakeWebContextWrapper());

			finder = new ItemFinder(sessionProvider, definitions);

			mocks.VerifyAll();
		}

		private static void SetConfigurationProperties(DefaultConfigurationBuilder configurationBuilder)
		{
			configurationBuilder.Properties[NHibernate.Cfg.Environment.ConnectionProvider] = "NHibernate.Connection.DriverConnectionProvider";
			configurationBuilder.Properties[NHibernate.Cfg.Environment.ConnectionStringName] = "TestConnection";

			configurationBuilder.Properties[NHibernate.Cfg.Environment.UseSecondLevelCache] = "false";
			configurationBuilder.Properties[NHibernate.Cfg.Environment.ConnectionDriver] = "NHibernate.Driver.SqlClientDriver";
			configurationBuilder.Properties[NHibernate.Cfg.Environment.Dialect] = "NHibernate.Dialect.MsSql2005Dialect";
		}

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			IRepository<int, ContentItem> itemRepository = new NHRepository<int, ContentItem>(sessionProvider);
			INHRepository<int, LinkDetail> linkRepository = new NHRepository<int, LinkDetail>(sessionProvider);

			persister = new DefaultPersister(itemRepository, linkRepository, finder);
			versioner = new VersionManager(persister, itemRepository);
		}

		[Test]
		public void SaveVersion()
		{
			ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
			persister.Save(item);
			ContentItem version = versioner.SaveVersion(item);
			Assert.AreEqual(item, version.VersionOf);
			persister.Delete(item);
		}

		[Test]
		public void RestoreVersion()
		{
			ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
			item["VersionIndex"] = 1;
			persister.Save(item);
			ContentItem version = versioner.SaveVersion(item);
			item["VersionIndex"] = 2;
			persister.Save(item);

			versioner.ReplaceVersion(item, version);

			ContentItem restoredItem = persister.Get(item.ID);
			Assert.AreEqual(1, restoredItem["VersionIndex"]);

			persister.Delete(item);
		}

		[Test]
		public void RestoreVersionSetExpireDate()
		{
			ContentItem item = CreateOneItem<Definitions.PersistableItem1>(0, "root", null);
			item["VersionIndex"] = 1;
			persister.Save(item);
			ContentItem version = versioner.SaveVersion(item);
			item["VersionIndex"] = 2;
			persister.Save(item);

			versioner.ReplaceVersion(item, version);

			ContentItem restoredItem = persister.Get(item.ID);
			Assert.IsNull(restoredItem.Expires, "Expires was supposed to be null but was " + restoredItem.Expires);

			persister.Delete(item);
		}
	}
}
