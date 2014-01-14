using System.Collections.Generic;
using N2.Collections;
using NUnit.Framework;

namespace N2.Tests.Collections
{
	[TestFixture]
	public class CountFilterTests : FilterTestsBase
	{
		public ItemList CreateItems(int numberOfItems)
		{
			ItemList items = new ItemList();
			for (int i = 0; i < numberOfItems; i++)
			{
				items.Add(CreateOneItem<FirstItem>(i, i.ToString(), null));
			}
			return items;
		}

		[Test]
		public void CanFilter_FromStart()
		{
			ItemList items = CreateItems(10);
			CountFilter filter = new CountFilter(0, 3);
			filter.Filter(items);
			Assert.AreEqual(3, items.Count);
			Assert.AreEqual(0, items[0].ID);
			Assert.AreEqual(1, items[1].ID);
			Assert.AreEqual(2, items[2].ID);
		}

		[Test]
		public void CanFilter_FromStart_WithStaticMethod()
		{
			ItemList items = CreateItems(10);
			CountFilter.Filter(items, 0, 3);
			Assert.AreEqual(3, items.Count);
			Assert.AreEqual(0, items[0].ID);
			Assert.AreEqual(1, items[1].ID);
			Assert.AreEqual(2, items[2].ID);
		}

		[Test]
		public void CanFilter_FromMid()
		{
			ItemList items = CreateItems(10);
			CountFilter filter = new CountFilter(4, 3);
			filter.Filter(items);
			Assert.AreEqual(3, items.Count);
			Assert.AreEqual(4, items[0].ID);
			Assert.AreEqual(5, items[1].ID);
			Assert.AreEqual(6, items[2].ID);
		}

		[Test]
		public void CanFilter_FromMid_WithStaticMethod()
		{
			ItemList items = CreateItems(10);
			CountFilter.Filter(items, 4, 3);
			Assert.AreEqual(3, items.Count);
			Assert.AreEqual(4, items[0].ID);
			Assert.AreEqual(5, items[1].ID);
			Assert.AreEqual(6, items[2].ID);
		}

		[Test]
		public void CanFilter_FromEnd()
		{
			ItemList items = CreateItems(10);
			CountFilter filter = new CountFilter(7, 3);
			filter.Filter(items);
			Assert.AreEqual(3, items.Count);
			Assert.AreEqual(7, items[0].ID);
			Assert.AreEqual(8, items[1].ID);
			Assert.AreEqual(9, items[2].ID);
		}

		[Test]
		public void CanFilter_FromEnd_WithStaticMethod()
		{
			ItemList items = CreateItems(10);
			CountFilter.Filter(items, 7, 3);
			Assert.AreEqual(3, items.Count);
			Assert.AreEqual(7, items[0].ID);
			Assert.AreEqual(8, items[1].ID);
			Assert.AreEqual(9, items[2].ID);
		}

		[Test]
		public void CanFilter_OneElement_FromEnd()
		{
			ItemList items = CreateItems(10);
			CountFilter filter = new CountFilter(9, 1);
			filter.Filter(items);
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(9, items[0].ID);
		}

		[Test]
		public void CanFilter_OneElement_FromEnd_WithStaticMethod()
		{
			ItemList items = CreateItems(10);
			CountFilter.Filter(items, 9, 1);
			Assert.AreEqual(1, items.Count);
			Assert.AreEqual(9, items[0].ID);
		}

		[Test]
		public void CanFilter_FromBeyondEnd()
		{
			ItemList items = CreateItems(10);
			CountFilter filter = new CountFilter(10, 3);
			filter.Filter(items);
			Assert.AreEqual(0, items.Count);
		}

		[Test]
		public void CanFilter_FromBeyondEnd_WithStaticMethod()
		{
			ItemList items = CreateItems(10);
			CountFilter.Filter(items, 10, 3);
			Assert.AreEqual(0, items.Count);
		}

		[Test]
		public void CanFilter_FromBeforeStart()
		{
			ItemList items = CreateItems(10);
			CountFilter filter = new CountFilter(-10, 3);
			filter.Filter(items);
			Assert.AreEqual(3, items.Count);
		}

		[Test]
		public void CanFilter_FromBeforeStart_WithStaticMethod()
		{
			ItemList items = CreateItems(10);
			CountFilter.Filter(items, -10, 3);
			Assert.AreEqual(3, items.Count);
		}

		[Test]
		public void CanFilter_FromEnd_WithInsufficientElements()
		{
			ItemList items = CreateItems(10);
			CountFilter filter = new CountFilter(7, 5);
			filter.Filter(items);
			Assert.AreEqual(3, items.Count);
			Assert.AreEqual(7, items[0].ID);
			Assert.AreEqual(8, items[1].ID);
			Assert.AreEqual(9, items[2].ID);
		}

		[Test]
		public void CanFilter_FromEnd_WithInsufficientElements_WithStaticMethod()
		{
			ItemList items = CreateItems(10);
			CountFilter.Filter(items, 7, 5);
			Assert.AreEqual(3, items.Count);
			Assert.AreEqual(7, items[0].ID);
			Assert.AreEqual(8, items[1].ID);
			Assert.AreEqual(9, items[2].ID);
		}

		[Test]
		public void CanFilter_ZeroElements_FromStart()
		{
			ItemList items = CreateItems(10);
			CountFilter filter = new CountFilter(0, 0);
			filter.Filter(items);
			Assert.AreEqual(0, items.Count);
		}

		[Test]
		public void CanFilter_ZeroElements_FromStart_WithStaticMethod()
		{
			ItemList items = CreateItems(10);
			CountFilter.Filter(items, 0, 0);
			Assert.AreEqual(0, items.Count);
		}

		[Test]
		public void CanFilter_ZeroElements_FromMid()
		{
			ItemList items = CreateItems(10);
			CountFilter filter = new CountFilter(5, 0);
			filter.Filter(items);
			Assert.AreEqual(0, items.Count);
		}

		[Test]
		public void CanFilter_ZeroElements_FromMid_WithStaticMethod()
		{
			ItemList items = CreateItems(10);
			CountFilter.Filter(items, 5, 0);
			Assert.AreEqual(0, items.Count);
		}

		[Test]
		public void CanFilter_ZeroElements_FromEnd()
		{
			ItemList items = CreateItems(10);
			CountFilter filter = new CountFilter(9, 0);
			filter.Filter(items);
			Assert.AreEqual(0, items.Count);
		}

		[Test]
		public void CanFilter_ZeroElements_FromEnd_WithStaticMethod()
		{
			ItemList items = CreateItems(10);
			CountFilter.Filter(items, 9, 0);
			Assert.AreEqual(0, items.Count);
		}

		[Test]
		public void CanFilter_EmptyList_FromStart()
		{
			ItemList items = CreateItems(0);
			CountFilter filter = new CountFilter(3, 2);
			filter.Filter(items);
			Assert.AreEqual(0, items.Count);
		}

		[Test]
		public void CanFilter_EmptyList_FromStart_WithStaticMethod()
		{
			ItemList items = CreateItems(0);
			CountFilter.Filter(items, 3, 2);
			Assert.AreEqual(0, items.Count);
		}

		[Test]
		public void CanFilter_TwoExcessiveItems()
		{
			ItemList fiveItems = CreateItems(5);
			CountFilter filter = new CountFilter(0, 3);
			filter.Filter(fiveItems);
			Assert.AreEqual(3, fiveItems.Count);
		}

		[Test]
		public void CanFilter_TwoExcessiveItems_WithStaticMethod()
		{
			ItemList fiveItems = CreateItems(5);
			CountFilter.Filter(fiveItems, 0, 3);
			Assert.AreEqual(3, fiveItems.Count);
		}

		[Test]
		public void CanFilter_FirstAndLastItem()
		{
			ItemList fiveItems = CreateItems(5);
			ContentItem toBeFirst = fiveItems[1];
			ContentItem toBeLast = fiveItems[3];
			CountFilter filter = new CountFilter(1, 3);
			filter.Filter(fiveItems);
			Assert.AreEqual(3, fiveItems.Count);
			Assert.AreEqual(toBeFirst, fiveItems[0]);
			Assert.AreEqual(toBeLast, fiveItems[2]);
		}

		[Test]
		public void CanFilter_FirstAndLastItem_WithStaticMethod()
		{
			ItemList fiveItems = CreateItems(5);
			ContentItem toBeFirst = fiveItems[1];
			ContentItem toBeLast = fiveItems[3];
			CountFilter.Filter(fiveItems, 1, 3);
			Assert.AreEqual(3, fiveItems.Count);
			Assert.AreEqual(toBeFirst, fiveItems[0]);
			Assert.AreEqual(toBeLast, fiveItems[2]);
		}

		[Test]
		public void CanFilter_FirstAndLastItems()
		{
			ItemList fiveItems = CreateItems(5);
			ContentItem toBeFirst = fiveItems[1];
			ContentItem toBeLast = fiveItems[3];
			CountFilter filter = new CountFilter(1, 3);
			filter.Filter(fiveItems);
			Assert.AreEqual(3, fiveItems.Count);
			Assert.AreEqual(toBeFirst, fiveItems[0]);
			Assert.AreEqual(toBeLast, fiveItems[2]);
		}

		[Test]
		public void CanFilter_FirstAndLastItems_WithStaticMethod()
		{
			ItemList fiveItems = CreateItems(5);
			ContentItem toBeFirst = fiveItems[1];
			ContentItem toBeLast = fiveItems[3];
			CountFilter.Filter(fiveItems, 1, 3);
			Assert.AreEqual(3, fiveItems.Count);
			Assert.AreEqual(toBeFirst, fiveItems[0]);
			Assert.AreEqual(toBeLast, fiveItems[2]);
		}

		[Test]
		public void CanPipe_FirstAndLastItems()
		{
			var fiveItems = CreateItems(5);
			CountFilter filter = new CountFilter(1, 3);
			IEnumerable<ContentItem> collection = filter.Pipe(fiveItems);
			var middleItems = new List<ContentItem>(collection);
			Assert.AreEqual(3, middleItems.Count);
			Assert.That(middleItems[0], Is.EqualTo(fiveItems[1]));
			Assert.That(middleItems[2], Is.EqualTo(fiveItems[3]));
		}

		[Test]
		public void CanSkipFilteringWithoutComplaining()
		{
			ItemList fiveItems = CreateItems(5);
			CountFilter filter = new CountFilter(0, 10);
			filter.Filter(fiveItems);
			Assert.AreEqual(5, fiveItems.Count);
		}

		[Test]
		public void CanSkipFilteringWithoutComplaining_WithStaticMethod()
		{
			ItemList fiveItems = CreateItems(5);
			CountFilter.Filter(fiveItems, 0, 10);
			Assert.AreEqual(5, fiveItems.Count);
		}

		[Test]
		public void CanFilterInExternalLoop()
		{
			ItemList fiveItems = CreateItems(5);
			CountFilter filter = new CountFilter(2, 2);

			int i = 0;
			Assert.IsFalse(filter.Match(fiveItems[i++]));
			Assert.IsFalse(filter.Match(fiveItems[i++]));
			Assert.IsTrue(filter.Match(fiveItems[i++]));
			Assert.IsTrue(filter.Match(fiveItems[i++]));
			Assert.IsFalse(filter.Match(fiveItems[i++]));
		}

		[Test]
		public void CanResetFilter()
		{
			ItemList fiveItems = CreateItems(5);
			CountFilter filter = new CountFilter(2, 2);

			foreach (ContentItem item in fiveItems)
				filter.Match(item);

			filter.Reset();
			int i = 0;
			Assert.IsFalse(filter.Match(fiveItems[i++]));
			Assert.IsFalse(filter.Match(fiveItems[i++]));
			Assert.IsTrue(filter.Match(fiveItems[i++]));
			Assert.IsTrue(filter.Match(fiveItems[i++]));
			Assert.IsFalse(filter.Match(fiveItems[i++]));
		}
	}
}
