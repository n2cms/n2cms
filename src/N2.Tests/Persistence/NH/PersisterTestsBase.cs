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
	public class PersisterTestsBase : ItemTestsBase
	{
		protected IDefinitionManager definitions;
		protected ContentPersister persister;
		protected SessionProvider sessionProvider;
		protected IItemFinder finder;

		[TestFixtureSetUp]
		public virtual void TestFixtureSetup()
		{
			mocks = new MockRepository();

			ITypeFinder typeFinder = mocks.StrictMock<ITypeFinder>();
			Expect.On(typeFinder)
				.Call(typeFinder.GetAssemblies())
				.Return(new Assembly[] { typeof(Definitions.PersistableItem1).Assembly })
				.Repeat.Any();
			Expect.On(typeFinder)
				.Call(typeFinder.Find(typeof(ContentItem)))
				.Return(new Type[] { typeof(Definitions.PersistableItem1) })
				.Repeat.AtLeastOnce();
			mocks.Replay(typeFinder);

			definitions = new DefinitionManager(new DefinitionBuilder(typeFinder, new EditableHierarchyBuilder<IEditable>(), new AttributeExplorer<EditorModifierAttribute>(), new AttributeExplorer<IDisplayable>(), new AttributeExplorer<IEditable>(), new AttributeExplorer<IEditableContainer>()), null);
			ConfigurationBuilder configurationBuilder = new ConfigurationBuilder(definitions);
			SetConfigurationProperties(configurationBuilder);

			sessionProvider = new SessionProvider(configurationBuilder, new Fakes.FakeWebContextWrapper());

			finder = new ItemFinder(sessionProvider, definitions);
		}

		private static void SetConfigurationProperties(ConfigurationBuilder configurationBuilder)
		{
			configurationBuilder.Properties["connection.provider"] = "NHibernate.Connection.DriverConnectionProvider";
			configurationBuilder.Properties["connection.connection_string_name"] = "TestConnection";

			configurationBuilder.Properties["cache.use_second_level_cache"] = "false";
			configurationBuilder.Properties["connection.driver_class"] = "NHibernate.Driver.SqlClientDriver";
			configurationBuilder.Properties["dialect"] = "NHibernate.Dialect.MsSql2005Dialect";
		}

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			IRepository<int, ContentItem> itemRepository = new NHRepository<int, ContentItem>(sessionProvider);
			INHRepository<int, LinkDetail> linkRepository = new NHRepository<int, LinkDetail>(sessionProvider);

			persister = new ContentPersister(itemRepository, linkRepository, finder);
		}

		[TearDown]
		public override void TearDown()
		{
			base.TearDown();
			persister.Dispose();
		}

	}
}
