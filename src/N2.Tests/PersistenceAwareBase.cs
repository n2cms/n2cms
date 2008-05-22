using NUnit.Framework;
using N2.Engine;
using N2.Installation;
using N2.Persistence.NH;
using N2.Security;

namespace N2.Tests
{
	public class PersistenceAwareBase : ItemTestsBase
	{
		protected IEngine engine;

		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
		{
			engine = new CmsEngine();
			engine.Resolve<ISecurityEnforcer>().Start();
		}

		[TearDown]
		public override void TearDown()
		{
			base.TearDown();
			engine.Persister.Dispose();
		}

		protected virtual DefaultPersister GetNHibernatePersistenceManager()
		{
			return engine.Persister as DefaultPersister;
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