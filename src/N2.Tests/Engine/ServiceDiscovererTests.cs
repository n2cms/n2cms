using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Engine;
using N2.Engine.MediumTrust;

namespace N2.Tests.Engine
{
    [TestFixture]
    public class ServiceDiscovererTests
    {
        [Service(Key = "Sesame")]
        class SelfService
        {
        }

        interface IService
        {
        }

        [Service(typeof(IService))]
        class InterfacedService : IService
        {
        }

        class NonAttributed
        {
        }

        [Test]
        public void AttributedClasses_AreAdded_ToTheContainer()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(SelfService), typeof(NonAttributed));
            MediumTrustServiceContainer container = new MediumTrustServiceContainer();

            ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
            registrator.Start();

            Assert.That(container.Resolve<SelfService>(), Is.InstanceOf<SelfService>());
            Assert.That(new TestDelegate(() => container.Resolve<NonAttributed>()), Throws.Exception);
        }

        [Test]
        public void AttributedClasses_AreAdded_ToTheContainer_WithServiceType()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(InterfacedService), typeof(NonAttributed));
            MediumTrustServiceContainer container = new MediumTrustServiceContainer();

            ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
            registrator.Start();

            Assert.That(container.Resolve<IService>(), Is.Not.Null);
            Assert.That(container.Resolve<IService>(), Is.InstanceOf<InterfacedService>());
            Assert.That(new TestDelegate(() => container.Resolve<NonAttributed>()), Throws.Exception);
        }

        [Test, Ignore("TODO")]
        public void Key_IsAssociated_WithService()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(SelfService), typeof(InterfacedService), typeof(NonAttributed));
            MediumTrustServiceContainer container = new MediumTrustServiceContainer();

            ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
            registrator.Start();

            Assert.That(container.Resolve("Sesame"), Is.Not.Null);
            Assert.That(container.Resolve("Sesame"), Is.InstanceOf<SelfService>());
        }
    }
}
