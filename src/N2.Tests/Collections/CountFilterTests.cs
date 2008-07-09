using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using N2.Collections;
using NUnit.Framework.SyntaxHelpers;

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
		public void CanFilter_TwoExessiveItems_WithStaticMethod()
		{
			ItemList fiveItems = CreateItems(5);
			CountFilter.Filter(fiveItems, 0, 3);
			Assert.AreEqual(3, fiveItems.Count);
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
		public void CanFilter_TwoExessiveItems_WithClassInstance()
		{
			ItemList fiveItems = CreateItems(5);
			CountFilter filter = new CountFilter(0, 3);
			filter.Filter(fiveItems);
			Assert.AreEqual(3, fiveItems.Count);
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
