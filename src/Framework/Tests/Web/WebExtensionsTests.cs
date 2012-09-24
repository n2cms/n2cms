using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using N2.Web;

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
        public void ShouldThrowExceptionIfEnumDoesNotHaveFlags()
        {
            var testEnum = TestEnumWithoutFlags.I;
            var testEnum2 = TestEnumWithoutFlags.Have;

            Assert.Throws<ArgumentException>(() => testEnum.IsFlagSet(testEnum2));
        }

        [Test]
        public void ShouldReturnTrueIfFlagIsSet()
        {
            var testEnum = TestEnumWithFlags.I | TestEnumWithFlags.Have;

            Assert.IsTrue(testEnum.IsFlagSet(TestEnumWithFlags.I));
            Assert.IsTrue(testEnum.IsFlagSet(TestEnumWithFlags.Have));
            Assert.IsFalse(testEnum.IsFlagSet(TestEnumWithFlags.Flags));
        }
    }
}
