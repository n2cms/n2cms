using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using N2.Engine.MediumTrust;
using N2.Persistence;
using NUnit.Framework.SyntaxHelpers;
using System.Configuration;
using System.Net.Mail;

namespace N2.Tests.MediumTrust
{
	[TestFixture]
	public class EngineTests
	{
		MediumTrustEngine engine;

		[SetUp]
		public void SetUp()
		{
			engine = new MediumTrustEngine();
		}

		[Test]
		public void CanInstantiate_MediumTrustEngine()
		{
			Assert.That(engine, Is.Not.Null);
		}

		[Test]
		public void CanResolve_AddedComponent()
		{
			engine.AddComponent("my.component", typeof(MyComponent));

			var sc = engine.Resolve<MyComponent>();
			Assert.That(sc, Is.Not.Null);
		}

		[Test]
		public void CanResolve_AddedComponent_WithDependencies()
		{
			engine.AddComponent("my.component", typeof(MyComponentWithDependencies));

			var mc = engine.Resolve<MyComponentWithDependencies>();
			Assert.That(mc, Is.Not.Null);
			Assert.That(mc.persister, Is.Not.Null);
		}

		public class MyComponent
		{
		}

		public class MyComponentWithDependencies
		{
			public IPersister persister;
			public MyComponentWithDependencies(IPersister persister)
			{
				this.persister = persister;
			}
		}
	}
}
