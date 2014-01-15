using N2.Engine;
using N2.Engine.MediumTrust;
using N2.Persistence;
using NUnit.Framework;
using N2.Web;

namespace N2.Tests.MediumTrust
{
    [TestFixture]
    public class EngineTests
    {
        IEngine engine;

        [SetUp]
        public void SetUp()
        {
            engine = new ContentEngine(new MediumTrustServiceContainer(), new EventBroker(), new ContainerConfigurer());
        }

        [Test]
        public void CanInstantiate_MediumTrustEngine()
        {
            Assert.That(engine, Is.Not.Null);
        }

        [Test]
        public void CanResolve_AddedComponent()
        {
            engine.Container.AddComponent("my.component", typeof(MyService), typeof(MyService));

            var sc = engine.Resolve<MyService>();
            Assert.That(sc, Is.Not.Null);
        }

        [Test]
        public void CanResolve_AddedComponent_WithDependencies()
        {
            engine.Container.AddComponent("my.component", typeof(MyClient), typeof(MyClient));

            var mc = engine.Resolve<MyClient>();
            Assert.That(mc, Is.Not.Null);
            Assert.That(mc.persister, Is.Not.Null);
        }

        [Test, Ignore("TODO")]
        public void CanInject_DependencyProperty()
        {
            engine.Container.AddComponent("my.service", typeof(MyService), typeof(MyService));
            engine.Container.AddComponent("my.component", typeof(MyClientWithProperty), typeof(MyClientWithProperty));

            var mc = engine.Resolve<MyClientWithProperty>();
            Assert.That(mc, Is.Not.Null);
            Assert.That(mc.Service, Is.Not.Null);
        }

        public class MyService
        {
        }

        public class MyClientWithProperty
        {
            public MyService Service { get; set; }
        }

        public class MyClient
        {
            public IPersister persister;
            public MyClient(IPersister persister)
            {
                this.persister = persister;
            }
        }
    }
}
