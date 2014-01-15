using System.Linq;
using N2.Tests.Collections;
using NUnit.Framework;

namespace N2.Tests.Persistence
{
    [TestFixture]
    public class FindTests : ItemTestsBase
    {
        #region SetUp
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            BuildHierarchy();
        }

        ContentItem a, a_a, a_b, a_a_a, a_a_b, a_b_a, a_b_b;
        ContentItem a_a_a_a, a_a_a_b, a_a_a_c, a_a_a_d, a_a_a_e;

        private void BuildHierarchy()
        {
            int i = 0;
            a = CreateOneItem<FirstItem>(++i, "a", null);
            a_a = CreateOneItem<FirstItem>(++i, "a_a", a);
            a_b = CreateOneItem<FirstItem>(++i, "a_b", a);

            a_a_a = CreateOneItem<FirstItem>(++i, "a_a_a", a_a);
            a_a_b = CreateOneItem<FirstItem>(++i, "a_a_b", a_a);

            a_a_a_a = CreateOneItem<FirstItem>(++i, "a_a_a_a", a_a_a);
            a_a_a_b = CreateOneItem<FirstItem>(++i, "a_a_a_b", a_a_a);
            a_a_a_c = CreateOneItem<FirstItem>(++i, "a_a_a_c", a_a_a);
            a_a_a_d = CreateOneItem<FirstItem>(++i, "a_a_a_d", a_a_a);
            a_a_a_e = CreateOneItem<FirstItem>(++i, "a_a_a_e", a_a_a);

            a_b_a = CreateOneItem<FirstItem>(++i, "a_b_a", a_b);
            a_b_b = CreateOneItem<FirstItem>(++i, "a_b_b", a_b);
        }
        #endregion

        // /a
        // /a/a_a
        // /a/a_a/a_a_a
        // /a/a_a/a_a_b
        // /a/a_b
        // /a/a_b/a_b_a
        // /a/a_b/a_b_b

        [Test]
        public void EnumerateParents_FindDoesNotPassLastItem()
        {
            EnumerableAssert.Count(1, Find.EnumerateParents(a_a_a, a_a));
        }

        [Test]
        public void EnumerateParents_CanAcceptNullAsLastItem()
        {
            EnumerableAssert.Count(2, Find.EnumerateParents(a_a_a, null));
        }

        [Test]
        public void EnumerateParents_FindsParentsOnTwoLevels()
        {
            EnumerableAssert.Count(2, Find.EnumerateParents(a_a_a, null));
        }

        [Test]
        public void EnumerateParents_FindDoesNotPassLastItem_WhenInitialItemIsLast()
        {
            EnumerableAssert.Count(0, Find.EnumerateParents(a_a, a_a));
        }

        [Test]
        public void EnumerateParents_CanIncludeSelf_Same_InitialAndLast()
        {
            EnumerableAssert.Count(1, Find.EnumerateParents(a_a, a_a, true));
        }

        [Test]
        public void EnumerateParents_CanIncludeSelf_Different_InitialAndLast()
        {
            EnumerableAssert.Count(2, Find.EnumerateParents(a_a, a, true));
        }

        [Test]
        public void EnumerateParents_YieldsNoParents_WhenGivenANullInitialItems()
        {
            Assert.That(Find.EnumerateParents(null, a_a).Count(), Is.EqualTo(0));
        }

        [Test]
        public void IsDescendant_ReturnsTrue_WhenGivenGrandParent()
        {
            bool isDescendant = Find.IsDescendant(a_a_a, a);
            Assert.IsTrue(isDescendant);
        }

        [Test]
        public void IsDescendant_ReturnsTrue_WhenGivenParent()
        {
            bool isDescendant = Find.IsDescendant(a_a_a, a_a);
            Assert.IsTrue(isDescendant);
        }

        [Test]
        public void IsDescendant_ReturnsFalse_WhenGivenItself()
        {
            bool isDescendant = Find.IsDescendant(a_a_a, a_a_a);
            Assert.IsFalse(isDescendant);
        }

        [Test]
        public void IsDescendant_ReturnsFalse_WhenGivenItemInOtherBranch()
        {
            bool isDescendant = Find.IsDescendant(a_a_a, a_b_a);
            Assert.IsFalse(isDescendant);
        }

        [Test]
        public void IsDescendant_ReturnsFalse_WhenGivenItemBelowItself()
        {
            bool isDescendant = Find.IsDescendant(a_a, a_a_a);
            Assert.IsFalse(isDescendant);
        }

        [Test]
        public void IsDescendantOrSelf_ReturnsTrue_WhenGivenItself()
        {
            bool isDescendant = Find.IsDescendantOrSelf(a_a_a, a_a_a);
            Assert.IsTrue(isDescendant);
        }

        [Test]
        public void IsDescendantOrSelf_ReturnsTrue_WhenGivenParent()
        {
            bool isDescendant = Find.IsDescendantOrSelf(a_a_a, a_a);
            Assert.IsTrue(isDescendant);
        }

        [Test]
        public void In_ReturnsTrue_WhenItemIsInEnumeration()
        {
            bool isIn = Find.In(a, new ContentItem[] {a_a_a, a_a, a, a_b, a_b_b});
            Assert.IsTrue(isIn);
        }

        [Test]
        public void In_ReturnsFalse_WhenItemIsNotInEnumeration()
        {
            bool isIn = Find.In(a_a_b, new ContentItem[] { a_a_a, a_a, a, a_b, a_b_b });
            Assert.IsFalse(isIn);
        }

        [Test]
        public void AtLevel_FindsItem_InTheMiddle()
        {
            ContentItem found = Find.AtLevel(a_a_a, a, 2);
            Assert.That(found, Is.EqualTo(a_a));
        }

        [Test]
        public void AtLevel_FindsItem_WhenStartingPoint_IsInTheMiddle()
        {
            ContentItem found = Find.AtLevel(a_a, a, 2);
            Assert.That(found, Is.EqualTo(a_a));
        }

        [Test]
        public void AtLevel_ReturnsNull_WhenStartingPoint_IsBelowCurrent()
        {
            ContentItem found = Find.AtLevel(a, a, 2);
            Assert.That(found, Is.Null);
        }

        [Test]
        public void AtLevel_FindsItem_SameAs_StartingPoint()
        {
            ContentItem found = Find.AtLevel(a_a_a, a, 3);
            Assert.That(found, Is.EqualTo(a_a_a));
        }

        [Test]
        public void AtLevel_FindsItem_SameAs_Root()
        {
            ContentItem found = Find.AtLevel(a_a_a, a, 1);
            Assert.That(found, Is.EqualTo(a));
        }

        [Test]
        public void AtLevel_ReturnsNull_WhenOutSideBounds_Upper()
        {
            ContentItem found = Find.AtLevel(a_a_a, a, 4);
            Assert.That(found, Is.Null);
        }

        [Test]
        public void AtLevel_ReturnsNull_WhenOutSideBounds_Lower()
        {
            ContentItem found = Find.AtLevel(a_a_a, a, 0);
            Assert.That(found, Is.Null);
        }

        [Test]
        public void EnumerateSiblings_ReturnsAllSiblings()
        {
            var found = Find.EnumerateSiblings(a_a_a_c);
            Assert.That(found.Count(), Is.EqualTo(5));
            Assert.That(found.Contains(a_a_a_a));
            Assert.That(found.Contains(a_a_a_b));
            Assert.That(found.Contains(a_a_a_c));
            Assert.That(found.Contains(a_a_a_d));
            Assert.That(found.Contains(a_a_a_e));
        }

        [Test]
        public void EnumerateSiblings_SupportsLonelyChild()
        {
            var found = Find.EnumerateSiblings(a);
            Assert.That(found.Count(), Is.EqualTo(0));
        }

        [Test]
        public void EnumerateSiblings_SiblingsBefore()
        {
            var found = Find.EnumerateSiblings(a_a_a_c, 1, 0);
            Assert.That(found.Count(), Is.EqualTo(2));
            Assert.That(found.Contains(a_a_a_b));
            Assert.That(found.Contains(a_a_a_c));
        }

        [Test]
        public void EnumerateSiblings_SiblingsBefore_DoesntGoOutOfBounds()
        {
            var found = Find.EnumerateSiblings(a_a_a_c, 5, 0);
            Assert.That(found.Count(), Is.EqualTo(3));
            Assert.That(found.Contains(a_a_a_a));
            Assert.That(found.Contains(a_a_a_b));
            Assert.That(found.Contains(a_a_a_c));
        }

        [Test]
        public void EnumerateSiblings_SiblingsAfter()
        {
            var found = Find.EnumerateSiblings(a_a_a_c, 0, 1);
            Assert.That(found.Count(), Is.EqualTo(2));
            Assert.That(found.Contains(a_a_a_c));
            Assert.That(found.Contains(a_a_a_d));
        }

        [Test]
        public void EnumerateSiblings_SiblingsAfter_DoesntGoOutOfBounds()
        {
            var found = Find.EnumerateSiblings(a_a_a_c, 0, 5);
            Assert.That(found.Count(), Is.EqualTo(3));
            Assert.That(found.Contains(a_a_a_c));
            Assert.That(found.Contains(a_a_a_d));
            Assert.That(found.Contains(a_a_a_e));
        }

        [Test]
        public void EnumerateSiblings_Adjecants()
        {
            var found = Find.EnumerateSiblings(a_a_a_c, 1, 1);
            Assert.That(found.Count(), Is.EqualTo(3));
            Assert.That(found.Contains(a_a_a_b));
            Assert.That(found.Contains(a_a_a_c));
            Assert.That(found.Contains(a_a_a_d));
        }

        [Test]
        public void EnumerateSiblings_Adjecants_DoesntGoOutOfBounds()
        {
            var found = Find.EnumerateSiblings(a_a_a_c, 5, 5);
            Assert.That(found.Count(), Is.EqualTo(5));
        }
    }
}
