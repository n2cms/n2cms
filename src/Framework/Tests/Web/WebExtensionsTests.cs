using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using N2.Web;
using Shouldly;
using N2.Tests.Fakes;
using System.IO;

namespace N2.Tests.Web
{
    [TestFixture]
    public class WebExtensionsTests
    {
        struct TestStruct
        {
            int AnInteger;
        }

        enum TestEnumWithoutFlags
        {
            I = 1,
            Have = 2,
            No  = 4,
            Flags = 8
        }

        [Flags]
        enum TestEnumWithFlags
        {
            I = 1,
            Have = 2,
            Flags = 4
        }

        [Test]
        public void ShouldThrowExceptionIfNotEnum()
        {
            var testStruct = new TestStruct();
            var testStruct2 = new TestStruct();

            Assert.Throws<ArgumentException>(() => testStruct.IsFlagSet(testStruct2));
        }

        [Test]
        public void ShouldReturnTrueIfFlagIsSet()
        {
            var testEnum = TestEnumWithFlags.I | TestEnumWithFlags.Have;

            testEnum.IsFlagSet(TestEnumWithFlags.I).ShouldBe(true);
            testEnum.IsFlagSet(TestEnumWithFlags.Have).ShouldBe(true);
            testEnum.IsFlagSet(TestEnumWithFlags.Flags).ShouldBe(false);
        }

		[Test]
		public void PostingJson_GetRequestValueAccessor_UsesJsonAsSource()
		{
			var ctx = new FakeHttpContext();
			ctx.request.CreatePost("/", "application/json", "{ hello: 'world' }");

			var accessor = ctx.GetRequestValueAccessor();

			accessor("hello").ShouldBe("world");
		}

		[Test]
		public void PostingJson_GetRequestValueAccessor_ConvertsJsonObjects_ToString()
		{
			var ctx = new FakeHttpContext();
			ctx.request.CreatePost("/", "application/json", "{ hello: 'world', one: 1 }");

			var accessor = ctx.GetRequestValueAccessor();

			accessor("one").ShouldBe("1");
		}
    }
}
