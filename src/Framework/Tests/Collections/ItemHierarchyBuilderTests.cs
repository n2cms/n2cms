using System;
using N2.Collections;
using NUnit.Framework;

namespace N2.Tests.Collections
{
    [TestFixture, Obsolete]
    public class ItemHierarchyBuilderTests : ItemTestsBase
    {
        #region SetUp

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            BuildHierarchy();
        }

        private ContentItem a, a_a, a_b, a_a_a, a_a_b, a_b_a, a_b_b;

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

        #endregion

        [Test]
        public void CanBuild_HierarchyBranch()
        {
            HierarchyNode<ContentItem> node = new BranchHierarchyBuilder(a_a_a, null).Build();
            ItemHierarchyNavigator ih = new ItemHierarchyNavigator(node);
            Assert.AreEqual(a, ih.Current);
            Assert.IsNull(ih.Parent);
            EnumerableAssert.Count(5, ih.EnumerateAllItems());
        }

        [Test]
        public void CanBuild_HierarchyBranch_AndAddInitialsChildren_WithSame_InitialAsLast()
        {
            HierarchyNode<ContentItem> node = new BranchHierarchyBuilder(a, a, true).Build();
            ItemHierarchyNavigator ih = new ItemHierarchyNavigator(node);
            EnumerableAssert.Count(3, ih.EnumerateAllItems());
        }

        [Test]
        public void CanBuild_HierarchyBranch_WithDifferent_InitialAsLast()
        {
            HierarchyNode<ContentItem> node = new BranchHierarchyBuilder(a_a, a).Build();
            ItemHierarchyNavigator ih = new ItemHierarchyNavigator(node);
            EnumerableAssert.Count(3, ih.EnumerateAllItems());
        }

        [Test]
        public void CanBuild_HierarchyBranch_AndAddInitialsChildren_WithDifferent_InitialAsLast()
        {
            HierarchyNode<ContentItem> node = new BranchHierarchyBuilder(a_a, a, true).Build();
            ItemHierarchyNavigator ih = new ItemHierarchyNavigator(node);
            EnumerableAssert.Count(5, ih.EnumerateAllItems());
        }

        [Test]
        public void CanBuild_Complete_HierarchyTree()
        {
            HierarchyNode<ContentItem> node = new TreeHierarchyBuilder(a).Build();
            ItemHierarchyNavigator ih = new ItemHierarchyNavigator(node);
            EnumerableAssert.Count(7, ih.EnumerateAllItems());
        }

        [Test]
        public void CanBuild_Partial_HierarchyTree()
        {
            HierarchyNode<ContentItem> node = new TreeHierarchyBuilder(a_a).Build();
            ItemHierarchyNavigator ih = new ItemHierarchyNavigator(node);
            EnumerableAssert.Count(3, ih.EnumerateAllItems());
        }

        [Test]
        public void BuildHierarchyTree_OtherNodes_AreNotIncluded()
        {
            HierarchyNode<ContentItem> node = new TreeHierarchyBuilder(a_b).Build();
            ItemHierarchyNavigator ih = new ItemHierarchyNavigator(node);
            EnumerableAssert.DoesntContain(ih.EnumerateAllItems(), a);
            EnumerableAssert.DoesntContain(ih.EnumerateAllItems(), a_a);
            EnumerableAssert.DoesntContain(ih.EnumerateAllItems(), a_a_a);
        }

        [Test]
        public void BuildHierarchyTree_CanTree_Depth1()
        {
            HierarchyNode<ContentItem> node = new TreeHierarchyBuilder(a, 1).Build();
            ItemHierarchyNavigator ih = new ItemHierarchyNavigator(node);
            EnumerableAssert.Count(1, ih.EnumerateAllItems());
        }

        [Test]
        public void BuildHierarchyTree_CanTree_Depth2()
        {
            HierarchyNode<ContentItem> node = new TreeHierarchyBuilder(a, 2).Build();
            ItemHierarchyNavigator ih = new ItemHierarchyNavigator(node);
            EnumerableAssert.Count(3, ih.EnumerateAllItems());
        }

        [Test]
        public void BuildHierarchyTree_CanTree_Depth3()
        {
            HierarchyNode<ContentItem> node = new TreeHierarchyBuilder(a, 3).Build();
            ItemHierarchyNavigator ih = new ItemHierarchyNavigator(node);
            EnumerableAssert.Count(7, ih.EnumerateAllItems());
        }

        [Test]
        public void BuildHierarchyBranch_SameInitialAndLast()
        {
            HierarchyNode<ContentItem> node = new BranchHierarchyBuilder(a, a).Build();
            ItemHierarchyNavigator ih = new ItemHierarchyNavigator(node);
            EnumerableAssert.Count(1, ih.EnumerateAllItems());
        }

        [Test]
        public void HierarchyNode_CanWrite_SingleNode_ToString()
        {
            HierarchyNode<ContentItem> node = new HierarchyNode<ContentItem>(a);

            var result = node.ToString((c) => "[" + c.ID, (p) => "<", p => ">", (c) => "]");
            Assert.That(result, Is.EqualTo("[1]"));
        }

        [Test]
        public void HierarchyNodeCanWrite_Hierarchy_ToString()
        {
            HierarchyNode<ContentItem> node = new TreeHierarchyBuilder(a, 2).Build();

            var result = node.ToString((c) => "[" + c.ID, (p) => "<", p => ">", (c) => "]");
            Assert.That(result, Is.EqualTo("[1<[2][3]>]"));
        }

        [Test]
        public void Parent_IsSet()
        {
            HierarchyNode<ContentItem> node = new TreeHierarchyBuilder(a, 3).Build();

            Assert.That(node.Parent, Is.Null);
            Assert.That(node.Children[0].Parent, Is.EqualTo(node));
            Assert.That(node.Children[0].Children[0].Parent, Is.EqualTo(node.Children[0]));
        }
    }
}
