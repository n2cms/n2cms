using N2.Collections;
using NUnit.Framework;

namespace N2.Tests.Collections
{
    [TestFixture]
    public class DuplicateFilterTests : FilterTestsBase
    {
        [Test]
        public void CanRemoveTwoDuplicatesWithStaticMethod()
        {
            ContentItem item = CreateOneItem<FirstItem>(1, "one", null);
            ItemList list = new ItemList();
            list.Add(item);
            list.Add(item);
            DuplicateFilter.FilterDuplicates(list);
            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public void CanRemoveTwoDuplicatesWithWithFilterInstance()
        {
            ContentItem item = CreateOneItem<FirstItem>(1, "one", null);
            ItemList list = new ItemList();
            list.Add(item);
            list.Add(item);
            DuplicateFilter filter = new DuplicateFilter();
            ((ItemFilter)filter).Filter(list);
            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public void CanRemoveSeveralDuplicatesWithWithFilterInstance()
        {
            ContentItem item = CreateOneItem<FirstItem>(1, "one", null);
            ContentItem item2 = CreateOneItem<FirstItem>(2, "two", null);
            ContentItem item3 = CreateOneItem<FirstItem>(3, "three", null);
            ItemList list = new ItemList();
            list.Add(item);
            list.Add(item2);
            list.Add(item2);
            list.Add(item3);
            list.Add(item3);
            list.Add(item3);
            
            DuplicateFilter filter = new DuplicateFilter();
            ((ItemFilter)filter).Filter(list);
            Assert.AreEqual(3, list.Count);
        }


        [Test]
        public void CanClearDuplicateFilter()
        {
            ContentItem item = CreateOneItem<FirstItem>(1, "one", null);
            ItemList list = new ItemList();
            list.Add(item);
            list.Add(item);
            list.Add(item);

            DuplicateFilter filter = new DuplicateFilter();
            filter.Filter(list);
            Assert.AreEqual(1, list.Count);
            filter.Clear();
            filter.Filter(list);
            Assert.AreEqual(1, list.Count);
        }
    }
}
