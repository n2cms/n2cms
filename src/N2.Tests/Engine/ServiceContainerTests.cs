using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Engine.Castle;
using N2.Engine.MediumTrust;
using N2.Engine;
using N2.Tests.Engine.Services;
using N2.Plugin;

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
			#region IAutoStart Members

			public void Start()
			{
				startOrder = ++counter;
				started = true;
			}

			public void Stop()
			{
				stopped = true;
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

		[Test]
		public void AutoStartServices_AreStarted()
		{
			container.AddComponent("key", typeof(Startable), typeof(Startable));
			container.StartComponents();
			var s = container.Resolve<Startable>();

			Assert.That(s.started, Is.True);
		}

		[Test]
		public void AutoStartServices_AreStarted_WhenEvenAfterStartSignal()
		{
			container.StartComponents();
			container.AddComponent("key", typeof(Startable), typeof(Startable));
			var s = container.Resolve<Startable>();

			Assert.That(s.started, Is.True);
		}
	}
}
