using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Engine.Castle;
using N2.Engine.MediumTrust;
using N2.Engine;
using N2.Tests.Engine.Services;

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
		public void RegisterService()
		{
			container.AddComponent("key", typeof(IService), typeof(InterfacedService));

			var service = container.Resolve<IService>();

			Assert.That(service, Is.InstanceOf<InterfacedService>());
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
	}
}
