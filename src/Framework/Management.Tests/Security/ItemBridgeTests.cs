using System;
using N2.Configuration;
using N2.Definitions;
using N2.Details;
using N2.Engine;
using N2.Management.Myself;
using N2.Management.Tests.Security.Items;
using N2.Persistence;
using N2.Persistence.NH;
using N2.Persistence.NH.Finder;
using N2.Persistence.Proxying;
using N2.Security;
using N2.Security.Items;
using N2.Tests.Fakes;
using N2.Web;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using Rhino.Mocks;

namespace N2.Tests.Security
{
    [TestFixture]
    public class ItemBridgeTests : ItemTestsBase
    {
        ItemBridge bridge;

        protected ContentActivator activator;
        protected IDefinitionManager definitions;
        protected ContentPersister persister;
        protected FakeSessionProvider sessionProvider;
        protected ItemFinder finder;
        protected SchemaExport schemaCreator;
        protected IItemNotifier notifier;
        protected InterceptingProxyFactory proxyFactory;
        protected Type[] persistedTypes = new[] { typeof(RootBase), typeof(UserList), typeof(User), typeof(UserOverride) };
        
        [TestFixtureSetUp]
        public virtual void TestFixtureSetUp()
        {
            TestSupport.Setup(out definitions, out activator, out notifier, out sessionProvider, out finder, out schemaCreator, out proxyFactory, persistedTypes);
            var repository = new ContentItemRepository(sessionProvider);
            var sources = TestSupport.SetupContentSource(repository);
            persister = new ContentPersister(sources, repository);
        }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            schemaCreator.Execute(/*script*/false, /*export*/true, /*justDrop*/false, sessionProvider.OpenSession.Connection, null);

			var root = new N2.Management.Myself.RootBase();
			persister.Save(root);
			bridge = new ItemBridge(activator, persister, new SecurityManager(new FakeWebContextWrapper(), new EditSection()), new Host(new FakeWebContextWrapper(), new HostSection { RootID = root.ID }), new EditSection { Membership = new MembershipElement { UserType = typeof(UserOverride).AssemblyQualifiedName } });
		}

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();

            persister.Dispose();
            sessionProvider.CloseConnections();
        }

        [Test]
        public void Create()
        {
            var u = bridge.CreateUser("test", "test", "", "", "", true, Guid.NewGuid());
            Assert.That(u.ID, Is.GreaterThan(0));
        }

        [Test]
        public void Create_Uses_UserOverride()
        {
            var u = bridge.CreateUser("test", "test", "", "", "", true, Guid.NewGuid());

            Assert.That(u, Is.InstanceOf<UserOverride>());
        }

        [Test]
        public void GetUser_GetsCreatedUser()
        {
            var createdUser = bridge.CreateUser("test", "test", "", "", "", true, Guid.NewGuid());
            var retrievedUser = bridge.GetUser("test");

            Assert.That(createdUser.ID, Is.EqualTo(retrievedUser.ID));
        }
    }
}
