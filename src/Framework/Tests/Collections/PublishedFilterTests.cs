using System;
using N2.Collections;
using NUnit.Framework;

namespace N2.Tests.Collections
{
    [TestFixture]
    public class PublishedFilterTests : FilterTestsBase
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
        public void CanFilterUnpublished()
        {
            ItemList list = CreateList();
            list[0].Published = N2.Utility.CurrentTime().AddDays(1);
            (new PublishedFilter()).Filter(list);
            Assert.AreEqual(2, list.Count);
        }

        [Test]
        public void CanFilterUnpublishedNull()
        {
            ItemList list = CreateList();
            list[0].Published = null;
            (new PublishedFilter()).Filter(list);
            Assert.AreEqual(2, list.Count);
        }

        [Test]
        public void CanFilterExpired()
        {
            ItemList list = CreateList();
            list[0].Expires = N2.Utility.CurrentTime().AddDays(-1);
            (new PublishedFilter()).Filter(list);
            Assert.AreEqual(2, list.Count);
        }
    }
}
