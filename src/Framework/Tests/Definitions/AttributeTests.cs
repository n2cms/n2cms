using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Shouldly;

namespace N2.Tests.Definitions
{
	[TestFixture]
	public class AttributeTests
	{
		[Inherited]
		[Uninherited]
		[AllowMultiple]
		[DisallowMultiple]
		public class A { }

		public class B : A { }

		[Inherited]
		[Uninherited]
		[AllowMultiple]
		[DisallowMultiple]
		public class C : B { }

		[AttributeUsage(AttributeTargets.Class, Inherited = true)]
		public class Inherited : Attribute { }

		[AttributeUsage(AttributeTargets.Class, Inherited = false)]
		public class Uninherited : Attribute { }

		[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
		public class AllowMultiple : Attribute { }

		[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
		public class DisallowMultiple : Attribute { }

		[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
		public class BaseA : Attribute { }
		public class SuperA : BaseA { }

		[SuperA]
		[BaseA]
		public class X { }
		[SuperA]
		[BaseA]
		public class Y : X { }

		Type at = typeof(A);
		Type bt = typeof(B);
		Type ct = typeof(C);
		Type xt = typeof(X);
		Type yt = typeof(Y);

		[Test]
		public void TestAll()
		{
			at.GetCustomAttributes(true).Length.ShouldBe(4);
			at.GetCustomAttributes(false).Length.ShouldBe(4);
			
			bt.GetCustomAttributes(true).Length.ShouldBe(3);
			bt.GetCustomAttributes(true).OfType<Uninherited>().ShouldBeEmpty();
			bt.GetCustomAttributes(false).Length.ShouldBe(0);

			ct.GetCustomAttributes(true).Length.ShouldBe(5);
			ct.GetCustomAttributes(true).OfType<AllowMultiple>().Count().ShouldBe(2);
			ct.GetCustomAttributes(false).Length.ShouldBe(4);

			xt.GetCustomAttributes(true).Length.ShouldBe(2);
			yt.GetCustomAttributes(true).Length.ShouldBe(3);
			yt.GetCustomAttributes(true).OfType<SuperA>().Count().ShouldBe(1);

		}
	}
}
