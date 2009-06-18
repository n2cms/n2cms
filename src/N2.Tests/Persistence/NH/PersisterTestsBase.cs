using N2.Tests.Fakes;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using N2.Definitions;
using N2.Details;
using N2.Engine;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Persistence.NH;
using N2.Persistence.NH.Finder;
using System.Configuration;
using N2.Configuration;

namespace N2.Tests.Persistence.NH
{
	public abstract class PersisterTestsBase : ItemTestsBase
	{
		protected IDefinitionManager definitions;
		protected ContentPersister persister;
		protected FakeSessionProvider sessionProvider;
		protected IItemFinder finder;
		protected SchemaExport schemaCreator;

		[TestFixtureSetUp]
		public virtual void TestFixtureSetup()
		{
			ITypeFinder typeFinder = new Fakes.FakeTypeFinder(typeof (Definitions.PersistableItem1).Assembly, typeof (Definitions.PersistableItem1));

			DefinitionBuilder definitionBuilder = new DefinitionBuilder(typeFinder, new EngineSection());
			definitions = new DefinitionManager(definitionBuilder, null);
			DatabaseSection config = (DatabaseSection)ConfigurationManager.GetSection("n2/database");
			ConnectionStringsSection connectionStrings = (ConnectionStringsSection)ConfigurationManager.GetSection("connectionStrings");
			ConfigurationBuilder configurationBuilder = new ConfigurationBuilder(definitions, config, connectionStrings);

			NotifyingInterceptor interceptor = new NotifyingInterceptor(new ItemNotifier());
			FakeWebContextWrapper context = new Fakes.FakeWebContextWrapper();
			sessionProvider = new FakeSessionProvider(new ConfigurationSource(configurationBuilder), interceptor, context);

			finder = new ItemFinder(sessionProvider, definitions);

			schemaCreator = new SchemaExport(configurationBuilder.BuildConfiguration());
		}

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			IRepository<int, ContentItem> itemRepository = new ContentItemRepository(sessionProvider);
			INHRepository<int, LinkDetail> linkRepository = new NHRepository<int, LinkDetail>(sessionProvider);

			persister = new ContentPersister(itemRepository, linkRepository, finder);

#if NH2_1
			schemaCreator.Execute(false, true, false, sessionProvider.OpenSession.Session.Connection, null);
#else
			schemaCreator.Execute(false, true, false, false, sessionProvider.OpenSession.Session.Connection, null);
#endif
		}

		[TearDown]
		public override void TearDown()
		{
			persister.Dispose();
			sessionProvider.CloseConnections();

			base.TearDown();
		}

	}
}
