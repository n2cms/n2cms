using System;
using System.Collections;
using NUnit.Framework;

namespace N2.Tests
{
    public static class EnumerableAssert
    {
        public static void Count(int count, IEnumerable list)
        {
            int i = 0;
            foreach (object o in list)
                ++i;
            Assert.AreEqual(count, i, "Expected list count " + count + " but was " + i);
        }

        public static void Contains(IEnumerable list, object o)
        {
            foreach (object listObject in list)
            {
                if (o.Equals(listObject))
                    return;
            }
            Assert.Fail("The list " + list + " didn't contain object " + o);
        }

        public static void DoesntContain(IEnumerable list, object o, string message)
        {
            foreach (object listObject in list)
            {
                if (o == null && listObject != null)
                    continue;
                if (o.Equals(listObject))
                    Assert.Fail(message);
            }
        }

        public static void DoesntContain(IEnumerable list, object o)
        {
            DoesntContain(list, o, "The list " + list + " did contain object " + o);
        }
    }

    [TestFixture]
    public class EnumerableAssertTests
    {
        [Test]
        public void Contains_Pass_OnCorrectCount()
        {
            EnumerableAssert.Count(2, new string[] { "one", "two" });
        }

        [Test]
        public void Count_Fails_OnInvalidCount()
        {
            ExceptionAssert.Throws<Exception>(delegate
            {
                EnumerableAssert.Count(3, new string[] { "one", "two" });
            });
        }

        [Test]
        public void Contains_Pass_OnExistingObject()
        {
            EnumerableAssert.Contains(new string[] { "one", "two" }, "one");
        }

        [Test]
        public void Contains_Fails_OnNonExistingObject()
        {
            ExceptionAssert.Throws<Exception>(delegate
            {
                EnumerableAssert.Contains(new string[] { "one", "two" }, "three");
            });
        }

        [Test]
        public void DoesntContain_Fails_OnExistingObject()
        {
            ExceptionAssert.Throws<Exception>(delegate
            {
                EnumerableAssert.DoesntContain(new string[] { "one", "two" }, "one");
            });
        }

        [Test]
        public void DoesntContain_Pass_OnNonExistingObject()
        {
            EnumerableAssert.DoesntContain(new string[] { "one", "two" }, "three");
        }
    }
}
