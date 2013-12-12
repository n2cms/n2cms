using System.Linq;
using N2.Engine;
using N2.Engine.Castle;
using N2.Engine.MediumTrust;
using N2.Plugin;
using N2.Tests.Engine.Services;
using NUnit.Framework;
using Shouldly;

namespace N2.Tests.Engine
{
    [TestFixture]
    public class WindsorServiceContainerTests : ServiceContainerTests
    {
        [SetUp]
        public void SetUp()
        {
            container = new WindsorServiceContainer();
        }
    }

    [TestFixture]
    public class MediumTrustServiceContainerTests : ServiceContainerTests
    {
        [SetUp]
        public void SetUp()
        {
            container = new MediumTrustServiceContainer();
        }
    }

    [TestFixture]
    public class TinyIoCServiceContainerTests : ServiceContainerTests
    {
        [SetUp]
        public void SetUp()
        {
            container = new N2.Engine.TinyIoC.TinyIoCServiceContainer();
        }
    }

    public abstract class ServiceContainerTests
    {
        protected IServiceContainer container;

        [Test]
        public void Resolve_Service_Generic()
        {
            container.AddComponent("key", typeof(IService), typeof(InterfacedService));

            var service = container.Resolve<IService>();

            Assert.That(service, Is.InstanceOf<InterfacedService>());
        }

        [Test]
        public void Resolve_Service_WithParameter()
        {
            container.AddComponent("key", typeof(IService), typeof(InterfacedService));

            var service = container.Resolve(typeof(IService));
            
            Assert.That(service, Is.InstanceOf<InterfacedService>());
        }

        [Test, Ignore]
        public void RegisteringService_AlsoRegisters_ConcreteType()
        {
            container.AddComponent("key", typeof(AbstractService), typeof(ConcreteService));

            var service = container.Resolve<ConcreteService>();

            Assert.That(service, Is.InstanceOf<ConcreteService>());
        }

        [Test]
        public void ResolveAll_SingleServices()
        {
            container.AddComponent("key", typeof(IService), typeof(InterfacedService));

            var services = container.ResolveAll<IService>();

            Assert.That(services.Count(), Is.EqualTo(1));
            Assert.That(services.First(), Is.InstanceOf<InterfacedService>());
        }

        [Test]
        public void ResolveAll_MultipleServices_Generic()
        {
            container.AddComponent("keyA", typeof(Startable), typeof(ServiceA));
            container.AddComponent("keyB", typeof(Startable), typeof(ServiceB));
            container.AddComponent("keyC", typeof(Startable), typeof(ServiceC));

            var services = container.ResolveAll<Startable>();

            Assert.That(services.Count(), Is.EqualTo(3));
            Assert.That(services.Any(s => s.GetType() == typeof(ServiceA)));
            Assert.That(services.Any(s => s.GetType() == typeof(ServiceB)));
            Assert.That(services.Any(s => s.GetType() == typeof(ServiceC)));
        }

        [Test]
        public void ResolveAll_MultipleServices_WithParameter()
        {
            container.AddComponent("keyA", typeof(Startable), typeof(ServiceA));
            container.AddComponent("keyB", typeof(Startable), typeof(ServiceB));
            container.AddComponent("keyC", typeof(Startable), typeof(ServiceC));

            var services = container.ResolveAll(typeof(Startable)).OfType<object>();

            Assert.That(services.Count(), Is.EqualTo(3));
            Assert.That(services.Any(s => s.GetType() == typeof(ServiceA)));
            Assert.That(services.Any(s => s.GetType() == typeof(ServiceB)));
            Assert.That(services.Any(s => s.GetType() == typeof(ServiceC)));
        }

        [Test, Ignore("TODO")]
        public void RegisterService_DecoratorChain()
        {
            container.AddComponent("decorator", typeof(IService), typeof(DecoratingService));
            container.AddComponent("decorated", typeof(IService), typeof(InterfacedService));

            var service = container.Resolve<IService>();

            Assert.That(service, Is.InstanceOf<DecoratingService>());
            Assert.That(((DecoratingService)service).decorated, Is.InstanceOf<InterfacedService>());
        }

        public class Startable : IAutoStart
        {
            public static int counter = 0;
            
            public int startOrder = 0;
            public bool started = false;
            public bool stopped = false;
            public int timesStarted = 0;
            public int timesStopped = 0;

            #region IAutoStart Members

            public void Start()
            {
                startOrder = ++counter;
                started = true;
                timesStarted++;
            }

            public void Stop()
            {
                stopped = true;
                timesStopped++;
            }

            #endregion
        }

        public class ServiceA : Startable
        {
        }

        public class ServiceB : Startable
        {
        }

        public class ServiceC : Startable
        {
        }

        public class ServiceABC
        {
            public Startable[] dependencies;

            public ServiceABC(Startable[] dependencies)
            {
                this.dependencies = dependencies;
            }
        }

        [Test]
        public void AutoStartServices_AreStarted()
        {
            container.AddComponent("key", typeof(Startable), typeof(Startable));
            container.StartComponents();
            var s = container.Resolve<Startable>();

            Assert.That(s.started, Is.True);
        }

        [Test]
        public void AutoStartServices_RetrieveBeforeGeneralStart_AreSameInstance()
        {
            container.AddComponent("key", typeof(IAutoStart), typeof(Startable));

            var s1 = container.Resolve<IAutoStart>();
            container.StartComponents();
            var s2 = container.Resolve<IAutoStart>();

            Assert.That(s1, Is.SameAs(s2));
        }

        [Test]
        public void AutoStartServices_AreStarted_WhenAddedAfterStartSignal()
        {
            container.StartComponents();
            container.AddComponent("key", typeof(IAutoStart), typeof(Startable));
            var s = (Startable)container.Resolve<IAutoStart>();

            Assert.That(s.started, Is.True);
        }

        [Test]
        public void AutoStartServices_AreStarted_Once()
        {
            container.AddComponent("key", typeof(IAutoStart), typeof(Startable));
            container.Resolve<IAutoStart>();
            container.StartComponents();
            container.Resolve<IAutoStart>();
            var s = (Startable)container.Resolve<IAutoStart>();

            Assert.That(s.timesStarted, Is.EqualTo(1));
        }

        [Test]
        public void CanDepend_OnArrayOfServices()
        {
            container.AddComponent("key1", typeof(Startable), typeof(ServiceA));
            container.AddComponent("key2", typeof(Startable), typeof(ServiceB));
            container.AddComponent("key3", typeof(Startable), typeof(ServiceC));
            container.AddComponent("key4", typeof(ServiceABC), typeof(ServiceABC));

            var service = container.Resolve<ServiceABC>();
            service.dependencies.Count().ShouldBe(3);
        }
    }
}
