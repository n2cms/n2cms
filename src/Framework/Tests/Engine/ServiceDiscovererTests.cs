using System.Linq;
using N2.Engine;
using N2.Engine.Castle;
using N2.Engine.MediumTrust;
using N2.Tests.Engine.Services;
using NUnit.Framework;
using System;
using N2.Web;
using N2.Configuration;
using Shouldly;
using System.Collections;
using System.Collections.Generic;

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

	[TestFixture]
	public class TinyIoCServiceDiscovererTests : ServiceDiscovererTests
	{
		[SetUp]
		public void SetUp()
		{
			container = new N2.Engine.TinyIoC.TinyIoCServiceContainer();
		}
	}

    public abstract class ServiceDiscovererTests
	{
        protected IServiceContainer container;

        [Test, Ignore("Not really required")]
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
			try
			{
				var instance = container.Resolve<NonAttributed>();
				Assert.Fail();
			}
			catch (Exception)
			{
				// expected
			}
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
		public void GenericServices_CanDependOn_2GenericInterfaces()
		{
			ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(GenericSelfService<>), typeof(GenericDependingOnInterfaceService));

			ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
			registrator.RegisterServices(registrator.FindServices());

			var service = container.Resolve<GenericDependingOnInterfaceService>();
			Assert.That(service, Is.InstanceOf<GenericDependingOnInterfaceService>());
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
			FindAndRegister(typeof(SelfService), typeof(DependingGenericSelfService<>));

			var service = container.Resolve<DependingGenericSelfService<string>>();
			Assert.That(service, Is.InstanceOf<DependingGenericSelfService<string>>());
			Assert.That(service.service, Is.InstanceOf<SelfService>());
		}

		[Test]
		public void CanResolve_MultipleServices()
		{
			FindAndRegister(typeof(HighService), typeof(LowService));

			var services = container.ResolveAll<IBarometer>();
			Assert.That(services.Count(), Is.EqualTo(2));
		}

		[Test]
		public void ResolveAll_GivesAnEnumeration_OfTheRequestedType()
		{
			FindAndRegister(typeof(HighService), typeof(LowService));

			var services = container.ResolveAll<IBarometer>();
			services.OfType<IBarometer>().Count().ShouldBe(2);
		}

		[Test]
		public void ResolveAll_WithTypeArgument_GivesAnArray_OfTheRequestedArrayType()
		{
			FindAndRegister(typeof(HighService), typeof(LowService));

			var services = container.ResolveAll(typeof(IBarometer));
			services.OfType<IBarometer>().Count().ShouldBe(2);
		}

		[Test]
		public void CanDependOn_MultipleServices()
		{
			FindAndRegister(typeof(HighService), typeof(LowService));
			container.AddComponent("bw", typeof(BarometerWatcher), typeof(BarometerWatcher));

			var watcher = container.Resolve<BarometerWatcher>();
			Assert.That(watcher.Barometers.Count(), Is.EqualTo(2));
		}

		[Test]
		public void CanDependOn_MultipleServices_WhenNoServicesAvailable()
		{
			container.AddComponent("bw", typeof(BarometerWatcher), typeof(BarometerWatcher));

			var watcher = container.Resolve<BarometerWatcher>();
			Assert.That(watcher.Barometers.Count(), Is.EqualTo(0));
		}

		[Test]
		public void Services_DependingOn_MultipleServices_CanBeSingletons()
		{
			FindAndRegister(typeof(HighService), typeof(LowService));
			container.AddComponent("bw", typeof(BarometerWatcher), typeof(BarometerWatcher));

			var watcher = container.Resolve<BarometerWatcher>();
			var watcher2 = container.Resolve<BarometerWatcher>();
			Assert.That(watcher, Is.SameAs(watcher));
		}

		[Test]
		public void CanRegister_MultipleServices_DependingOnSame_ServiceArray()
		{
			ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(HighService), typeof(LowService));
			ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
			registrator.RegisterServices(registrator.FindServices());
			container.AddComponent("bw", typeof(BarometerWatcher), typeof(BarometerWatcher));
			container.AddComponent("ac", typeof(AltitudeComparer), typeof(AltitudeComparer));

			var watcher = container.Resolve<BarometerWatcher>();
			var comparer = container.Resolve<AltitudeComparer>();

			Assert.That(watcher.Barometers[0], Is.SameAs(comparer.Barometers[0]));
			Assert.That(watcher.Barometers[1], Is.SameAs(comparer.Barometers[1]));
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
		public void CanResolve_ServiceWithDependency_OnComponentInstance()
		{
			ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(DependingServiceWithMissingDependency).Assembly.GetTypes());
			ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
			registrator.RegisterServices(registrator.FindServices());

			container.AddComponentInstance("ud", typeof(UnregisteredDependency), new UnregisteredDependency());

			var service = container.Resolve<DependingServiceWithMissingDependency>();
			Assert.That(service, Is.InstanceOf<DependingServiceWithMissingDependency>());
		}

		[Test]
		public void Resolves_OnlyRequestedConfiguration()
		{
			ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(HighService), typeof(LowService));

			ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
			var services = registrator.FilterServices(registrator.FindServices(), "High");
			registrator.RegisterServices(services);

			Assert.That(container.Resolve<IBarometer>(), Is.InstanceOf<HighService>());
			Assert.That(container.ResolveAll<IBarometer>().Count(), Is.EqualTo(1));
		}

		[Test]
		public void Requesting_MultipleConfigurations_GivesAllMatchingServices()
		{
			ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(HighService), typeof(LowService));

			ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
			var services = registrator.FilterServices(registrator.FindServices(), "High", "Medium", "Low");
			registrator.RegisterServices(services);

			Assert.That(container.ResolveAll<IBarometer>().Count(), Is.EqualTo(2));
		}

		[Test]
		public void Requesting_NoConfigurations_DoesntResolveServices_ThatUsesConfigurations()
		{
			ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(HighService), typeof(LowService));

			ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
			var services = registrator.FilterServices(registrator.FindServices());
			registrator.RegisterServices(services);

			Assert.That(container.ResolveAll<IBarometer>().Count(), Is.EqualTo(0));
		}

		[Test]
		public void RequstingConfiguration_AlsoRegisterd_ServicesWithoutConfiguration()
		{
			ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(SelfService), typeof(HighService), typeof(LowService));

			ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
			registrator.RegisterServices(registrator.FilterServices(registrator.FindServices(), "High"));

			Assert.That(container.Resolve<SelfService>(), Is.InstanceOf<SelfService>());
			Assert.That(container.Resolve<IBarometer>(), Is.InstanceOf<HighService>());
			Assert.That(container.ResolveAll<IBarometer>().Count(), Is.EqualTo(1));
		}

		[Test]
		public void Services_CanOverride_OtherServices()
		{
			ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(InterfacedService), typeof(ReplacingService));

			ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
			registrator.RegisterServices(registrator.FilterServices(registrator.FindServices()));

			Assert.That(container.Resolve<IService>(), Is.InstanceOf<ReplacingService>());
			Assert.That(container.ResolveAll<IService>().Count(), Is.EqualTo(1));
		}

		[Test]
		public void Services_CanOverride_OtherServices_ByServiceType()
		{
			ITypeFinder finder = new Fakes.FakeTypeFinder(typeof(ReplacedService), typeof(ReplacingReplacedService));

			ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
			registrator.RegisterServices(registrator.FilterServices(registrator.FindServices()));

			Assert.That(container.Resolve<IReplacedInterface>(), Is.InstanceOf<ReplacingReplacedService>());
			Assert.That(container.ResolveAll<IReplacedInterface>().Count(), Is.EqualTo(1));
		}

		[Test]
		public void InternalServices_AreFound()
		{
			var finder = TestSupport.TypeFinder();

			var registrator = new ServiceRegistrator(finder, container);
			registrator.RegisterServices(registrator.FindServices());

			container.Resolve<IInternalService>().ShouldBeTypeOf<InternalService>();
		}

		[Test]
		public void NestedServices_AreFound()
		{
			var finder = TestSupport.TypeFinder();

			var registrator = new ServiceRegistrator(finder, container);
			registrator.RegisterServices(registrator.FindServices());

			container.Resolve<OuterService.InnerService>().ShouldNotBe(null);
		}

		[Test]
		public void NestedNewServices_AreFound()
		{
			var finder = TestSupport.TypeFinder();

			var registrator = new ServiceRegistrator(finder, container);
			registrator.RegisterServices(registrator.FindServices());

			container.Resolve<OuterService2.InnerService>().ShouldNotBe(null);
		}

		private void FindAndRegister(params Type[] types)
		{
			ITypeFinder finder = new Fakes.FakeTypeFinder(types);
			ServiceRegistrator registrator = new ServiceRegistrator(finder, container);
			registrator.RegisterServices(registrator.FindServices());
		}
	}
}
