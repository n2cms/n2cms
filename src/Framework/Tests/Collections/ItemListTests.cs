using N2.Collections;
using NUnit.Framework;

namespace N2.Tests.Collections
{
    [TestFixture]
    public class ItemListTests : ItemTestsBase
    {
        [Test]
        public void CanCastItemList()
        {
            ItemList items = new ItemList();
            items.Add(CreateOneItem<FirstItem>(1, "one", null));
            items.Add(CreateOneItem<SecondItem>(1, "two", null));

            ItemList<FirstItem> firsts = items.Cast<FirstItem>();
            Assert.That(firsts.Count, Is.EqualTo(1));
        }
    }
}
