using System;
using N2.Definitions;
using N2.Persistence;
using N2.Persistence.NH;
using N2.Persistence.NH.Finder;
using N2.Persistence.Proxying;
using N2.Tests.Fakes;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace N2.Tests.Persistence.NH
{
    public abstract class PersisterTestsBase : ItemTestsBase
    {
        protected ContentActivator activator;
        protected IDefinitionManager definitions;
        protected ContentPersister persister;
        protected FakeSessionProvider sessionProvider;
        protected SchemaExport schemaCreator;
        protected IItemNotifier notifier;
        protected InterceptingProxyFactory proxyFactory;
        protected Type[] persistedTypes = new[] { typeof(Definitions.PersistableItem), typeof(Definitions.PersistableItem2), typeof(Definitions.NonVirtualItem) };
            
        [TestFixtureSetUp]
        public virtual void TestFixtureSetup()
        {
            ItemFinder finder;
            TestSupport.Setup(out definitions, out activator, out notifier, out sessionProvider, out finder, out schemaCreator, out proxyFactory, persistedTypes);
        }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            TestSupport.Setup(out persister, sessionProvider, schemaCreator);
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
