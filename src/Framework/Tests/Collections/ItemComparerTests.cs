using N2.Collections;
using NUnit.Framework;

namespace N2.Tests.Collections
{
    [TestFixture]
    public class ItemComparerTests
    {
        FirstItem item1, item2;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            item1 = new FirstItem();
            item1.Title = "Title1";
            item1.SortOrder = 2;
            item2 = new FirstItem();
            item2.Title = "Title2";
            item2.SortOrder = 1;

        }

        [Test]
        public void DefaultComparer()
        {
            ItemComparer comparer = new ItemComparer();
            int comparisonResult = comparer.Compare(item1, item2);
            Assert.Less(0, comparisonResult, "The first item should have been greater than the second due to sort order.");
        }

        [Test]
        public void CompareTitle()
        {
            ItemComparer comparer = new ItemComparer("Title");
            int comparisonResult = comparer.Compare(item1, item2);
            Assert.Greater(0, comparisonResult);
        }

        [Test]
        public void CompareTitle2()
        {
            ItemComparer comparer = new ItemComparer("Title", false);
            int comparisonResult = comparer.Compare(item1, item2);
            Assert.Greater(0, comparisonResult);
        }

        [Test]
        public void CompareTitleDesc()
        {
            ItemComparer comparer = new ItemComparer("Title DESC");
            int comparisonResult = comparer.Compare(item1, item2);
            Assert.Less(0, comparisonResult);
        }

        [Test]
        public void CompareTitleDesc2()
        {
            ItemComparer comparer = new ItemComparer("Title", true);
            int comparisonResult = comparer.Compare(item1, item2);
            Assert.Less(0, comparisonResult);
        }

        [Test]
        public void CompareTitleStatic()
        {
            int comparisonResult = ItemComparer.Compare(item1, item2, "Title", false);
            Assert.Greater(0, comparisonResult);
        }

        [Test]
        public void CompareTitleStaticDesc()
        {
            int comparisonResult = ItemComparer.Compare(item1, item2, "Title", true);
            Assert.Less(0, comparisonResult);
        }
    }
}
