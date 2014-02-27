using System;
using N2.Collections;
using NUnit.Framework;

namespace N2.Tests.Collections
{
    [Obsolete]
    [TestFixture]
    public class ItemHierarchyTests : ItemTestsBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            BuildHierarchy();
        }

        ContentItem a, a_a, a_b, a_a_a, a_a_b, a_b_a, a_b_b;
        private void BuildHierarchy()
        {
            int i = 0;
            a = CreateOneItem<FirstItem>(++i, "a", null);
            a_a = CreateOneItem<FirstItem>(++i, "a_a", a);
            a_b = CreateOneItem<FirstItem>(++i, "a_b", a);

            a_a_a = CreateOneItem<FirstItem>(++i, "a_a_a", a_a);
            a_a_b = CreateOneItem<FirstItem>(++i, "a_a_b", a_a);

            a_b_a = CreateOneItem<FirstItem>(++i, "a_b_a", a_b);
            a_b_b = CreateOneItem<FirstItem>(++i, "a_b_b", a_b);
        }

        [Test]
        public void BuildsAllTheWayToTheRoot()
        {
            ItemHierarchyNavigator ih = new ItemHierarchyNavigator(new BranchHierarchyBuilder(a_a_a, null));
            Assert.AreEqual(a, ih.Current);
            Assert.IsNull(ih.Parent);
        }

        [Test]
        public void BuildsToRootWithRootAsLastItem()
        {
            ItemHierarchyNavigator ih = new ItemHierarchyNavigator(new BranchHierarchyBuilder(a_a_a, a));
            Assert.AreEqual(a, ih.Current);
            Assert.IsNull(ih.Parent);
        }

        [Test]
        public void BuildsToRootIfLastItemIsntA_Parent()
        {
            ItemHierarchyNavigator ih = new ItemHierarchyNavigator(new BranchHierarchyBuilder(a_a_a, a_b));
            Assert.AreEqual(a, ih.Current);
            Assert.IsNull(ih.Parent);
        }

        [Test]
        public void StopsAtLastItemEvenIfNotRoot()
        {
            ItemHierarchyNavigator ih = new ItemHierarchyNavigator(new BranchHierarchyBuilder(a_a_a, a_a));
            Assert.AreEqual(a_a, ih.Current);
            Assert.IsNull(ih.Parent);
        }

        [Test]
        public void AddsAllSiblings()
        {
            ItemHierarchyNavigator ih = new ItemHierarchyNavigator(new BranchHierarchyBuilder(a_a_a, null));
            EnumerableAssert.Contains(ih.EnumerateAllItems(), a_a_a);
            EnumerableAssert.Contains(ih.EnumerateAllItems(), a_a_b);
        }

        [Test]
        public void DoesntAddItemsInOffPaths()
        {
            ItemHierarchyNavigator ih = new ItemHierarchyNavigator(new BranchHierarchyBuilder(a_a_a, null));
            EnumerableAssert.Count(5, ih.EnumerateAllItems());
            EnumerableAssert.Contains(ih.EnumerateAllItems(), a_a_a);
            EnumerableAssert.DoesntContain(ih.EnumerateAllItems(), a_b_a);
        }

        [Test]
        public void CanFindRootLevelFromAllLevels()
        {
            ItemHierarchyNavigator ih = new ItemHierarchyNavigator(new BranchHierarchyBuilder(a_a_a, null));
            CompareItemsToRootRecursive(a, ih);
        }

        [Test]
        public void Parent_IsSet()
        {
            HierarchyNode<ContentItem> node = new BranchHierarchyBuilder(a_a_a, null).Build();

            Assert.That(node.Parent, Is.Null);
            Assert.That(node.Children[0].Parent, Is.EqualTo(node));
            Assert.That(node.Children[0].Children[0].Parent, Is.EqualTo(node.Children[0]));
        }

        [Test]
        public void CanBuildCompleteHierarchy()
        {
            ItemHierarchyNavigator ih = new ItemHierarchyNavigator(new TreeHierarchyBuilder(a));
            EnumerableAssert.Count(7, ih.EnumerateAllItems());
        }

        private static void CompareItemsToRootRecursive(ContentItem root, ItemHierarchyNavigator ih)
        {
            foreach (ItemHierarchyNavigator childHierarchy in ih.Children)
            {
                Assert.AreEqual(root, childHierarchy.GetRootHierarchy().Current);
                CompareItemsToRootRecursive(root, childHierarchy);
            }
        }
    }
}
