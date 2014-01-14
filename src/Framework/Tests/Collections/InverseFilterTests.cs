using N2.Collections;
using NUnit.Framework;

namespace N2.Tests.Collections
{
    [TestFixture]
    public class InverseFilterTests : FilterTestsBase
    {
        protected ItemList CreateList()
        {
            ItemList list = new ItemList();
            list.Add(CreateOneItem<FirstItem>(1, "one", null));
            list.Add(CreateOneItem<SecondItem>(2, "two", null));
            list.Add(CreateOneItem<NonPageItem>(3, "three", null));
            return list;
        }

        [Test]
        public void CanFilterPagesWithStaticMethod()
        {
            ItemList list = CreateList();
            InverseFilter.FilterInverse(list, new PageFilter());
            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public void CanFilterPagesWithClassInstance()
        {
            ItemList list = CreateList();
            (new InverseFilter(new PageFilter())).Filter(list);
            Assert.AreEqual(1, list.Count);
        }
    }
}
