using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using N2.Collections;

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
				items.Add(CreateOneItem<FirstItem>(i + 1, i.ToString(), null));
			}
			return items;
		}

		[Test]
		public void CanFilterTwoExessiveItemsWithStaticMethod()
		{
			ItemList fiveItems = CreateItems(5);
			CountFilter.Filter(fiveItems, 0, 3);
			Assert.AreEqual(3, fiveItems.Count);
		}

		[Test]
		public void CanFilterFirstAndLastItemWithStaticMethod()
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
		public void CanFilterTwoExessiveItemsWithClassInstance()
		{
			ItemList fiveItems = CreateItems(5);
			CountFilter filter = new CountFilter(0, 3);
			filter.Filter(fiveItems);
			Assert.AreEqual(3, fiveItems.Count);
		}

		[Test]
		public void CanFilterFirstAndLastItemsWithClassInstance()
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
		public void CanSkipFilteringWithoutComplaining()
		{
			ItemList fiveItems = CreateItems(5);
			CountFilter filter = new CountFilter(0, 10);
			filter.Filter(fiveItems);
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
