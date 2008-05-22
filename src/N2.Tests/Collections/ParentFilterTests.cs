using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using N2.Collections;
using NUnit.Framework.Extensions;

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

		[RowTest]
		[Row(1, 12)]
		[Row(2, 3)]
		[Row(3, 3)]
		[Row(4, 3)]
		[Row(5, 0)]
		public void CanFilterByParentID(int parentID, int expectedCount)
		{
			ParentFilter filter = new ParentFilter(parentID);
			filter.Filter(allItems);
			Assert.AreEqual(expectedCount, allItems.Count);
		}

		[Test]
		public void CanFilterByParentItem()
		{
			ParentFilter filter = new ParentFilter(child1);
			filter.Filter(allItems);
			Assert.AreEqual(3, allItems.Count);
		}
	}
}
