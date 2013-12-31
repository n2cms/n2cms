using N2.Collections;
using NUnit.Framework;

namespace N2.Tests.Collections
{
    [TestFixture]
    public class ZoneFilterTests : FilterTestsBase
    {
        protected ContentItem CreateItem<T>(int id, string zone) where T : ContentItem
        {
            T item = CreateOneItem<T>(id, id.ToString(), null);
            item.ZoneName = zone;
            return item;
        }

        [Test]
        public void CanFilterTwoItemsWithStaticMethod()
        {
            ItemList items = new ItemList();
            items.Add(CreateItem<FirstItem>(1, "Zone1"));
            items.Add(CreateItem<FirstItem>(2, "Zone1"));
            items.Add(CreateItem<FirstItem>(3, "Zone2"));
            items.Add(CreateItem<FirstItem>(4, "Zone2"));
            ZoneFilter.Filter(items, "Zone1");
            Assert.AreEqual(2, items.Count);
        }

        [Test]
        public void CanFilterTwoItemsWithClassInstance()
        {
            ItemList items = new ItemList();
            items.Add(CreateItem<FirstItem>(1, "Zone1"));
            items.Add(CreateItem<FirstItem>(2, "Zone1"));
            items.Add(CreateItem<FirstItem>(3, "Zone2"));
            items.Add(CreateItem<FirstItem>(4, "Zone2"));
            ZoneFilter filter = new ZoneFilter("Zone1");
            filter.Filter(items);
            Assert.AreEqual(2, items.Count);
        }
    }
}
