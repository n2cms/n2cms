using MbUnit.Framework;
using N2.Collections;

namespace N2.Tests.Collections
{
	[TestFixture]
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
		public void CanBuildHierarchyBranch()
		{
			HierarchyNode<ContentItem> node = new BranchHierarchyBuilder(a_a_a, null).Build();
			ItemHierarchyNavigator ih = new ItemHierarchyNavigator(node);
			Assert.AreEqual(a, ih.Current);
			Assert.IsNull(ih.Parent);
			EnumerableAssert.Count(5, ih.EnumerateAllItems());
		}

		[Test]
		public void CanBuildCompleteHierarchyTree()
		{
			HierarchyNode<ContentItem> node = new TreeHierarchyBuilder(a).Build();
			ItemHierarchyNavigator ih = new ItemHierarchyNavigator(node);
			EnumerableAssert.Count(7, ih.EnumerateAllItems());
		}

		[Test]
		public void CanBuildPartialHierarchyTree()
		{
			HierarchyNode<ContentItem> node = new TreeHierarchyBuilder(a_a).Build();
			ItemHierarchyNavigator ih = new ItemHierarchyNavigator(node);
			EnumerableAssert.Count(3, ih.EnumerateAllItems());
		}

		[Test]
		public void BuildHierarchyTree_OtherNodesAreNotIncluded()
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
	}
}