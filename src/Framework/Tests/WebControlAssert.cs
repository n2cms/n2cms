using System;
using System.Web.UI;
using NUnit.Framework;

namespace N2.Tests
{
    public static class WebControlAssert
    {
        public static void Contains(Type controlType, Control container)
        {
            bool contains = ContainsRecursive(controlType, container);
            Assert.IsTrue(contains, "The control " + container.ID + " didn't contain any " + controlType);
        }
        public static void Contains(Control expected, Control container)
        {
            bool contains = ContainsRecursive(expected, container);
            Assert.IsTrue(contains, "The control " + container.ID + " didn't contain any " + expected.ID);
        }
        private static bool ContainsRecursive(Type controlType, Control container)
        {
            if(container.GetType() == controlType)
                return true;
            foreach (Control c in container.Controls)
            {
                if (ContainsRecursive(controlType, c))
                    return true;
            }
            return false;
        }
        private static bool ContainsRecursive(Control expected, Control container)
        {
            if (container == expected)
                return true;
            foreach (Control c in container.Controls)
            {
                if (ContainsRecursive(expected, c))
                    return true;
            }
            return false;
        }
    }
}
