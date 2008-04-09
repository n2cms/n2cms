using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using N2.MediumTrust.Engine;

namespace N2.Tests.MediumTrust
{
	[TestFixture]
	public class EngineTests
	{
		[Test]
		public void CanInstantiateEngine()
		{
			MediumTrustFactory engine = new MediumTrustFactory();
		}
	}
}
