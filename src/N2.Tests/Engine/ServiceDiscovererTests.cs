using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Engine;
using N2.Engine.MediumTrust;
using N2.Engine.Castle;
using N2.Persistence;
using N2.Persistence.NH;
using N2.Details;
using N2.Tests.Engine.Services;

namespace N2.Tests.Engine
{
	[TestFixture]
	public class MediumTrustServiceDiscovererTests : ServiceDiscovererTests
	{
		[SetUp]
		public void SetUp()
		{
			container = new MediumTrustServiceContainer();
		}
	}

	[TestFixture]
	public class WindsorServiceDiscovererTests : ServiceDiscovererTests
	{
		[SetUp]
		public void SetUp()
		{
			container = new WindsorServiceContainer();
		}
	}

    public abstract class ServiceDiscovererTests
	{
        protected IServiceContainer container;

        [Test]
        public void Services_AreAdded_ToTheContainer()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(SelfService), typeof(NonAttributed));

            ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
            registrator.RegisterServices(registrator.FindServices());

            Assert.That(container.Resolve<SelfService>(), Is.InstanceOf<SelfService>());
            Assert.That(new TestDelegate(() => container.Resolve<NonAttributed>()), Throws.Exception);
		}

		[Test]
		public void Services_CanDepend_OnEachOther()
		{
			ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(SelfService), typeof(DependingService));

			ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
			registrator.RegisterServices(registrator.FindServices());

			var service = container.Resolve<DependingService>();
			Assert.That(service, Is.InstanceOf<DependingService>());
			Assert.That(service.service, Is.InstanceOf<SelfService>());
		}

		[Test]
		public void Services_AreSingletons()
		{
			ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(SelfService));

			ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
			registrator.RegisterServices(registrator.FindServices());

			var one = container.Resolve<SelfService>();
			var two = container.Resolve<SelfService>();
			
			Assert.That(object.ReferenceEquals(one, two));
		}

        [Test]
        public void Services_AreAdded_ToTheContainer_WithServiceType()
        {
            ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(InterfacedService), typeof(NonAttributed));

            ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
            registrator.RegisterServices(registrator.FindServices());

            Assert.That(container.Resolve<IService>(), Is.Not.Null);
            Assert.That(container.Resolve<IService>(), Is.InstanceOf<InterfacedService>());
            Assert.That(new TestDelegate(() => container.Resolve<NonAttributed>()), Throws.Exception);
        }

		[Test]
		public void GenericServices_CanBeResolved()
		{
			ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(GenericSelfService<>));

			ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
			registrator.RegisterServices(registrator.FindServices());

			Assert.That(container.Resolve<GenericSelfService<int>>(), Is.InstanceOf<GenericSelfService<int>>());
			Assert.That(container.Resolve<GenericSelfService<string>>(), Is.InstanceOf<GenericSelfService<string>>());
		}

		[Test]
		public void GenericServices_CanBeResolved_ByServiceInterface()
		{
			ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(GenericInterfacedService<>));

			ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
			registrator.RegisterServices(registrator.FindServices());

			Assert.That(container.Resolve<IGenericService<int>>(), Is.InstanceOf<GenericInterfacedService<int>>());
			Assert.That(container.Resolve<IGenericService<string>>(), Is.InstanceOf<GenericInterfacedService<string>>());
		}

		[Test]
		public void GenericServices_CanDepend_OnEachOther()
		{
			ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(GenericSelfService<>), typeof(GenericDependingService));

			ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
			registrator.RegisterServices(registrator.FindServices());

			var service = container.Resolve<GenericDependingService>();
			Assert.That(service, Is.InstanceOf<GenericDependingService>());
			Assert.That(service.service, Is.InstanceOf<GenericSelfService<int>>());
		}

		[Test]
		public void Services_CanDepend_OnGenericServiceInterface()
		{
			ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(GenericInterfaceDependingService), typeof(GenericInterfacedService<>));

			ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
			registrator.RegisterServices(registrator.FindServices());

			var service = container.Resolve<GenericInterfaceDependingService>();
			Assert.That(service, Is.InstanceOf<GenericInterfaceDependingService>());
			Assert.That(service.service, Is.InstanceOf<GenericInterfacedService<int>>());
		}

		[Test]
		public void GenericServices_CanDepend_OnService()
		{
			ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(SelfService), typeof(DependingGenericSelfService<>));

			ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
			registrator.RegisterServices(registrator.FindServices());

			var service = container.Resolve<DependingGenericSelfService<string>>();
			Assert.That(service, Is.InstanceOf<DependingGenericSelfService<string>>());
			Assert.That(service.service, Is.InstanceOf<SelfService>());
		}

		[Test, Ignore("TODO")]
		public void Service_CanDecorate_ServiceOfSameType()
		{
			ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(DecoratingService), typeof(InterfacedService));

			ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
			registrator.RegisterServices(registrator.FindServices());

			var service = container.Resolve<IService>();

			Assert.That(service, Is.InstanceOf<DecoratingService>());
			Assert.That(((DecoratingService)service).decorated, Is.InstanceOf<InterfacedService>());
		}

		[Test]
		public void X()
		{
			ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(ContentItem).Assembly.GetTypes());
			container.AddComponentInstance("x",
				typeof(IPersister), 
				new ContentPersister(new Fakes.FakeRepository<ContentItem>(), new Fakes.FakeRepository<LinkDetail>(), null));

			ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
			registrator.RegisterServices(registrator.FindServices());
			
			var service = container.Resolve<N2.Engine.StructureBoundDictionaryCache<int, string>>();
			Assert.That(service, Is.InstanceOf<N2.Engine.StructureBoundDictionaryCache<int, string>>());
		}
	}
}
