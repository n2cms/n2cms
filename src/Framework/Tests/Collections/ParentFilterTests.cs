using System.Collections.Generic;
using N2.Collections;
using NUnit.Framework;

namespace N2.Tests.Collections
{
    [TestFixture]
    public class ParentFilterTests : FilterTestsBase
    {
        #region SetUp
        ContentItem root;
        ContentItem child1;
        ContentItem child2;
        ContentItem child3;
        List<ContentItem> allItems;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            allItems = new List<ContentItem>();
            int i = 0;

            allItems.Add(root = CreateOneItem<FirstItem>(++i, "root", null));

            allItems.Add(child1 = CreateOneItem<FirstItem>(++i, "child1", root));
            allItems.Add(child2 = CreateOneItem<FirstItem>(++i, "child2", root));
            allItems.Add(child3 = CreateOneItem<FirstItem>(++i, "child3", root));

            allItems.Add(CreateOneItem<FirstItem>(++i, "child1_" + i, child1));
            allItems.Add(CreateOneItem<FirstItem>(++i, "child1_" + i, child1));
            allItems.Add(CreateOneItem<FirstItem>(++i, "child1_" + i, child1));

            allItems.Add(CreateOneItem<FirstItem>(++i, "child2_" + i, child2));
            allItems.Add(CreateOneItem<FirstItem>(++i, "child2_" + i, child2));
            allItems.Add(CreateOneItem<FirstItem>(++i, "child2_" + i, child2));

            allItems.Add(CreateOneItem<FirstItem>(++i, "child3_" + i, child3));
            allItems.Add(CreateOneItem<FirstItem>(++i, "child3_" + i, child3));
            allItems.Add(CreateOneItem<FirstItem>(++i, "child3_" + i, child3));
        }
        #endregion

        [TestCase(1, 12)]
        [TestCase(2, 3)]
        [TestCase(3, 3)]
        [TestCase(4, 3)]
        [TestCase(5, 0)]
        public void CanFilterByParentID(int parentID, int expectedCount)
        {
            AncestorFilter filter = new AncestorFilter(parentID);
            filter.Filter(allItems);
            Assert.AreEqual(expectedCount, allItems.Count);
        }

        [Test]
        public void CanFilterByParentItem()
        {
            AncestorFilter filter = new AncestorFilter(child1);
            filter.Filter(allItems);
            Assert.AreEqual(3, allItems.Count);
        }
    }
}
