using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using N2.MediumTrust.Engine;
using N2.Persistence;
using NUnit.Framework.SyntaxHelpers;

namespace N2.Tests.MediumTrust
{
	[TestFixture]
	public class EngineTests
	{
		[Test]
		public void CanInstantiateEngine()
		{
			MediumTrustEngine engine = new MediumTrustEngine();
		}

		[Test]
		public void CanResolve_AddedComponent()
		{
			MediumTrustEngine engine = new MediumTrustEngine();
			engine.AddComponent("my.component", typeof(MyComponent));

			MyComponent mc = engine.Resolve<MyComponent>();
			Assert.That(mc, Is.Not.Null);
			Assert.That(mc.persister, Is.Not.Null);
		}

		public class MyComponent
		{
			public IPersister persister;
			public MyComponent(IPersister persister)
			{
				this.persister = persister;
			}
		}
	}
}
