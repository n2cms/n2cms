using N2.Tests.Fakes;
using N2.Web.Targeting;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Tests.Web.Targeting
{
	class Always : DetectorBase
	{
		public override bool IsTarget(TargetingContext context)
		{
			return true;
		}
	}
	class Never : DetectorBase
	{
		public override bool IsTarget(TargetingContext context)
		{
			return false;
		}
	}
	class Settable : DetectorBase
	{
		private bool isTarget;

		public Settable(bool isTarget)
		{
			this.isTarget = isTarget;
		}
		
		public override bool IsTarget(TargetingContext context)
		{
			return isTarget;
		}
	}
	[TestFixture]
	public class TargetingTests
	{
		private TargetingRadar radar;
		private TargetingContext context;

		[SetUp]
		public void SetUp()
		{
			radar = new TargetingRadar(new DetectorBase[] { new Always(), new Never() });
			context = radar.BuildTargetingContext(new Fakes.FakeHttpContext());
		}

		[Test]
		public void IsTargetedBy_PositiveDetectors()
		{
			var ctx = radar.BuildTargetingContext(new Fakes.FakeHttpContext());
			ctx.TargetedBy.Single().ShouldBeTypeOf<Always>();
		}

		[TestCase("/world.aspx", "/Always/world.aspx")]
		[TestCase("/hello/world.aspx", "/hello/Always/world.aspx")]
		[TestCase("/Views/Shared/_Layout.cshtml", "/Views/Shared/Always/_Layout.cshtml")]
		[TestCase("/", "/Always")]
		public void TargeName_IsInserted_BeforeFileName(string original, string expected)
		{
			context.GetTargetedPaths(original).First().ShouldBe(expected);
		}

		[TestCase("/world.aspx", "/world.aspx")]
		[TestCase("/hello/world.aspx", "/hello/world.aspx")]
		[TestCase("/", "/")]
		public void LastTarget_IsOriginalPath(string original, string expected)
		{
			context.GetTargetedPaths(original).Last().ShouldBe(expected);
		}
	}
}
