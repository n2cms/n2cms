using N2.Collections;
using NUnit.Framework;

namespace N2.Tests.Collections
{
    [TestFixture]
    public class TypeFilterTests : FilterTestsBase
    {
        protected ItemList CreateList()
        {
            ItemList list = new ItemList();
            list.Add(CreateOneItem<FirstItem>(1, "one", null));
            list.Add(CreateOneItem<SecondItem>(2, "two", null));
            list.Add(CreateOneItem<SecondItem>(3, "three", null));
            return list;
        }

        [Test]
        public void CanFilterTwoItemsWithStaticMethod()
        {
            ItemList list = CreateList();
            TypeFilter.Filter(list, typeof(FirstItem));
            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public void CanFilterOneItemWithStaticMethod()
        {
            ItemList list = CreateList();
            TypeFilter.Filter(list, typeof(SecondItem));
            Assert.AreEqual(2, list.Count);
        }

        [Test]
        public void CanFilterTwoItemsWithFilterInstance()
        {
            ItemFilter filter = new TypeFilter(typeof(FirstItem));
            ItemList list = CreateList();
            filter.Filter(list);
            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public void CorrectItemMatches()
        {
            ItemFilter filter = new TypeFilter(typeof(FirstItem));
            Assert.IsTrue(filter.Match(new FirstItem()));
        }

        [Test]
        public void IncorrectItemDoesntMatch()
        {
            ItemFilter filter = new TypeFilter(typeof(FirstItem));
            Assert.IsFalse(filter.Match(new SecondItem()));
        }

        //[Test]
        //public void CanFilterTwoItemsWithStaticMethodAndInverse()
        //{
        //    ItemList list = CreateList();
        //    TypeFilter.Filter(true, list, typeof(FirstItem));
        //    Assert.AreEqual(2, list.Count);
        //}

        [Test]
        public void CanFilterTwoItemsWithFilterInstanceAndInverse()
        {
            ItemFilter filter = new InverseFilter(new TypeFilter(typeof(FirstItem)));
            ItemList list = CreateList();
            filter.Filter(list);
            Assert.AreEqual(2, list.Count);
        }
    }
}
