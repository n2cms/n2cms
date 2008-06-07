using NUnit.Framework;
using N2.Engine;
using N2.Installation;
using N2.Persistence.NH;
using N2.Security;
using System.Configuration;

namespace N2.Tests
{
	public class PersistenceAwareBase : ItemTestsBase
	{
		protected IEngine engine;

		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
		{
			engine = new ContentEngine(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None));
			engine.Resolve<ISecurityEnforcer>().Start();
		}

		[TearDown]
		public override void TearDown()
		{
			base.TearDown();
			engine.Persister.Dispose();
		}

		protected virtual ContentPersister GetNHibernatePersistenceManager()
		{
			return engine.Persister as ContentPersister;
		}

		protected virtual void CreateDatabaseSchema()
		{
			InstallationManager im = new InstallationManager(engine);
			im.Install();
		}

		protected virtual void DropDatabaseSchema()
		{
			InstallationManager im = new InstallationManager(engine);
			im.DropDatabaseTables();
		}
	}
}