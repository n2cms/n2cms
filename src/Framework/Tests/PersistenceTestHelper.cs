using N2.Engine;
using N2.Persistence.NH;
using N2.Tests.Fakes;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Tests
{
    public class PersistenceTestHelper
    {
        public IEngine Engine { get; set; }
        public SchemaExport SchemaCreator { get; set; }
        public FakeSessionProvider SessionProvider { get; set; }

        public virtual void TestFixtureSetUp()
        {
            Context.Replace(Engine = new ContentEngine());

            var configurationBuilder = Engine.Resolve<IConfigurationBuilder>();
            SessionProvider = (FakeSessionProvider)Engine.Resolve<ISessionProvider>();
            SchemaCreator = new SchemaExport(configurationBuilder.BuildConfiguration());
            CreateDatabaseSchema();

            Engine.Initialize();
        }

        public virtual void TearDown()
        {
            SessionProvider.CloseConnections();
        }

        public virtual void TestFixtureTearDown()
        {
            Context.Replace(null);
        }

        public virtual void CreateDatabaseSchema()
        {
            SessionProvider.CurrentSession = null;
            SchemaCreator.Execute(false, true, false, SessionProvider.OpenSession.Session.Connection, null);
        }

        public virtual void DropDatabaseSchema()
        {
            SchemaCreator.Execute(false, true, true, SessionProvider.OpenSession.Session.Connection, null);
        }
    }
}
