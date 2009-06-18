using System.Data;
using N2.Tests.Fakes;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using N2.Engine;
using N2.Installation;
using N2.Persistence.NH;
using N2.Security;
using System.Configuration;

namespace N2.Tests
{
	public abstract class PersistenceAwareBase : ItemTestsBase
	{
		protected IEngine engine;
		protected SchemaExport schemaCreator;
		protected FakeSessionProvider sessionProvider;

		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
		{
			var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			
			engine = new ContentEngine(config, "n2", EventBroker.Instance);
            engine.Initialize();
			engine.Resolve<ISecurityEnforcer>().Start();

			var configurationBuilder = engine.Resolve<IConfigurationBuilder>();
			sessionProvider = (FakeSessionProvider)engine.Resolve<ISessionProvider>();
			schemaCreator = new SchemaExport(configurationBuilder.BuildConfiguration());
		}

		[TearDown]
		public override void TearDown()
		{
			sessionProvider.CloseConnections();
			base.TearDown();
		}

		protected virtual ContentPersister GetNHibernatePersistenceManager()
		{
			return engine.Persister as ContentPersister;
		}

		protected virtual void CreateDatabaseSchema()
		{
#if NH2_1
			schemaCreator.Execute(false, true, false, sessionProvider.OpenSession.Session.Connection, null);
#else
			schemaCreator.Execute(false, true, false, false, sessionProvider.OpenSession.Session.Connection, null);
#endif
		}

		protected virtual void DropDatabaseSchema()
		{
#if NH2_1
			schemaCreator.Execute(false, true, true, sessionProvider.OpenSession.Session.Connection, null);
#else
			schemaCreator.Execute(false, true, true, false, sessionProvider.OpenSession.Session.Connection, null);
#endif
			
		}
	}
}