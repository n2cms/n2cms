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
using System;
using N2.Persistence.Proxying;

namespace N2.Tests.Persistence.NH
{
	public abstract class PersisterTestsBase : ItemTestsBase
	{
		protected IDefinitionManager definitions;
		protected ContentPersister persister;
		protected FakeSessionProvider sessionProvider;
		protected ItemFinder finder;
		protected SchemaExport schemaCreator;
		protected IItemNotifier notifier;
		protected InterceptingProxyFactory proxyFactory;
		protected Type[] persistedTypes = new[] { typeof(Definitions.PersistableItem1), typeof(Definitions.PersistableItem2), typeof(Definitions.NonVirtualItem) };
			
		[TestFixtureSetUp]
		public virtual void TestFixtureSetup()
		{
			TestSupport.Setup(out definitions, out notifier, out sessionProvider, out finder, out schemaCreator, out proxyFactory, persistedTypes);
		}

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

            TestSupport.Setup(out persister, sessionProvider, finder, schemaCreator);
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
