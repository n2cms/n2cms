using N2.Collections;
using NUnit.Framework;

namespace N2.Tests.Collections
{
    [TestFixture]
    public class VisibleFilterTests : FilterTestsBase
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
        public void CanFilterInvisible()
        {
            ItemList list = CreateList();
            list[0].Visible = false;
            (new VisibleFilter()).Filter(list);
            Assert.AreEqual(2, list.Count);
        }
    }
}
