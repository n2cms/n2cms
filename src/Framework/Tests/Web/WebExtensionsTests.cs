using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using N2.Web;
using Shouldly;

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
    }
}
