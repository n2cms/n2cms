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
            radar = new TargetingRadar(new N2.Configuration.HostSection(), new DetectorBase[] { new Always(), new Never() });
            context = radar.BuildTargetingContext(new Fakes.FakeHttpContext());
        }

        [Test]
        public void IsTargetedBy_PositiveDetectors()
        {
            var ctx = radar.BuildTargetingContext(new Fakes.FakeHttpContext());
			ctx.TargetedBy.Single().ShouldBeOfType<Always>();
        }

        [TestCase("/world.aspx", "/world_Always.aspx")]
        [TestCase("/hello/world.aspx", "/hello/world_Always.aspx")]
        [TestCase("/Views/Shared/_Layout.cshtml", "/Views/Shared/_Layout_Always.cshtml")]
        public void TargeName_IsInserted_AfterFileName(string original, string expected)
        {
            context.GetTargetedPaths(original).First().ShouldBe(expected);
        }

        [TestCase("/", "/Always/")]
        [TestCase("/Hello/", "/Hello/Always/")]
        public void TargetWithPath_AppendsTargetName_AsTrailingSegment(string original, string expected)
        {
            context.GetTargetedPaths(original).First().ShouldBe(expected);
        }


        [Test]
        public void PathWithoutExtensionOrTrailingSlash_IsIgnored()
        {
            context.GetTargetedPaths("/hello").Any().ShouldBe(false);
        }

        [TestCase("/world.aspx")]
        [TestCase("/hello/world.aspx")]
        [TestCase("/")]
        public void LastTarget_IsNotOriginalPath(string original)
        {
            context.GetTargetedPaths(original).Last().ShouldNotBe(original);
        }
    }
}
