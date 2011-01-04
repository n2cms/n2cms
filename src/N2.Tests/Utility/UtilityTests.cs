using System;
using System.Collections.Generic;
using NUnit.Framework;
using N2.Collections;
using N2.Integrity;

namespace N2.Tests.Utility
{
	[TestFixture]
	public class UtilityTests : ItemTestsBase
	{
		private ContentItem item1;
		private ContentItem item2;
		private ContentItem item3;
		private ContentItem item4;
		private ContentItem item5;

		private ContentItem[] items;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			int i = 0;
			item1 = CreateOneItem<UtilityItem>(++i, i.ToString(), null);
			item2 = CreateOneItem<UtilityItem>(++i, i.ToString(), null);
			item3 = CreateOneItem<UtilityItem>(++i, i.ToString(), null);
			item4 = CreateOneItem<UtilityItem>(++i, i.ToString(), null);
			item5 = CreateOneItem<UtilityItem>(++i, i.ToString(), null);

			items = new ContentItem[] {item1, item2, item3, item4, item5};
		}

		[Test]
		public void UpdateSortOrder_CanSetUniqueSortOrder()
		{
			Assert.AreEqual(0, item1.SortOrder);
			Assert.AreEqual(0, item2.SortOrder);
			Assert.AreEqual(0, item3.SortOrder);
			Assert.AreEqual(0, item4.SortOrder);
			Assert.AreEqual(0, item5.SortOrder);

			IEnumerable<ContentItem> changedItems = N2.Utility.UpdateSortOrder(items);

			EnumerableAssert.Count(4, changedItems);

			Assert.Less(item1.SortOrder, item2.SortOrder);
			Assert.Less(item2.SortOrder, item3.SortOrder);
			Assert.Less(item3.SortOrder, item4.SortOrder);
			Assert.Less(item4.SortOrder, item5.SortOrder);
		}

		[Test]
		public void UpdateSortOrder_DoesntChangeAlreadyOrderedItems()
		{
			item1.SortOrder = 1;
			item2.SortOrder = 2;
			item3.SortOrder = 3;
			item4.SortOrder = 4;
			item5.SortOrder = 5;

			IEnumerable<ContentItem> changedItems = N2.Utility.UpdateSortOrder(items);

			EnumerableAssert.Count(0, changedItems);
		}

		[Test]
		public void UpdateSortOrder_CanFixOrderOnLastItem()
		{
			item1.SortOrder = 1;
			item2.SortOrder = 2;
			item3.SortOrder = 3;
			item4.SortOrder = 4;
			item5.SortOrder = 0;

			IEnumerable<ContentItem> changedItems = N2.Utility.UpdateSortOrder(items);

			EnumerableAssert.Count(1, changedItems);
			Assert.Less(item4.SortOrder, item5.SortOrder);
		}

		[TestCase(0, 1)]
		[TestCase(0, 0)]
		[TestCase(0, 4)]
		[TestCase(2, 0)]
		[TestCase(2, 2)]
		[TestCase(2, 4)]
		[TestCase(4, 0)]
		[TestCase(4, 2)]
		[TestCase(4, 4)]
		public void MoteToIndex_Move(int initialIndex, int moveToIndex)
		{
			ItemList itemList = new ItemList(items);
			N2.Utility.MoveToIndex(itemList, itemList[initialIndex], moveToIndex);
			Assert.AreEqual(5, itemList.Count);
			Assert.AreEqual(items[initialIndex], itemList[moveToIndex]);
			if (initialIndex != moveToIndex)
			{
				Assert.AreNotEqual(items[moveToIndex], itemList[moveToIndex]);
				Assert.AreNotEqual(items[initialIndex], itemList[initialIndex]);
			}
		}

		[Test]
		public void CanEvaluateIntProperty()
		{
			object value = N2.Utility.Evaluate(item1, "ID");
			Assert.AreEqual(1, value);
		}

		[Test]
		public void CanEvaluate_IntProperty_AndApplyFormatString()
		{
			object value = N2.Utility.Evaluate(item1, "ID", "[{0}]");
			Assert.AreEqual("[1]", value);
		}

		[Test]
		public void CanEvaluateStringProperty()
		{
			object value = N2.Utility.Evaluate(item1, "Name");
			Assert.AreEqual("1", value);
		}

		[Test]
		public void CanEvaluateBoolProperty()
		{
			object value = N2.Utility.Evaluate(item1, "IsPage");
			Assert.AreEqual(true, value);
		}

		[Test]
		public void CanEvaluateContentItemProperty()
		{
			item1.AddTo(item2);
			object value = N2.Utility.Evaluate(item1, "Parent");
			Assert.AreEqual(item2, value);
		}

		[Test]
		public void CanEvaluateParentID()
		{
			item1.AddTo(item2);
			object value = N2.Utility.Evaluate(item1, "Parent.ID");
			Assert.AreEqual(2, value);
		}

		[Test]
		public void EvaluateNonExistantPropertyYeldsNull()
		{
			item1.AddTo(item2);
			object value = N2.Utility.Evaluate(item1, "ImNotThere");
			Assert.IsNull(value);
		}

		[Test]
		public void EvaluateNonExistantParentPropertyYeldsNull()
		{
			item1.AddTo(item2);
			object value = N2.Utility.Evaluate(item1, "Parent.ImNotThere");
			Assert.IsNull(value);
		}

		[Test]
		public void CanEvalueateAReallyLongExperssion()
		{
			item1.AddTo(item2);
			item2.AddTo(item3);
			item3.AddTo(item4);
			item4.AddTo(item5);
			object value = N2.Utility.Evaluate(item1, "Parent.Parent.Parent.Parent.ID");
			Assert.AreEqual(5, value);
		}

		[Test]
		public void EvaluateOverNullYeldsNull()
		{
			object value = N2.Utility.Evaluate(item1, "Parent.ID");
			Assert.IsNull(value);
		}

		[TestCase("123", 123, typeof (int))]
		[TestCase("True", true, typeof (bool))]
		[TestCase("false", false, typeof (bool))]
		[TestCase(123, "123", typeof (string))]
		[TestCase(true, "True", typeof (string))]
		[TestCase(false, "False", typeof (string))]
		public void CanConvert(object from, object expectedResult, Type destinationType)
		{
			object result = N2.Utility.Convert(from, destinationType);
			Assert.AreEqual(expectedResult, result);
		}

		[Test]
		public void CanSetValue()
		{
			N2.Utility.SetProperty(item1, "Name", "WhooHoo");
			Assert.AreEqual("WhooHoo", item1.Name);
		}

		[Test]
		public void CanSetValueOfWrongType()
		{
			N2.Utility.SetProperty(item1, "Name", 123);
			Assert.AreEqual("123", item1.Name);
		}

		[Test]
		public void CanGetProperty()
		{
			object value = N2.Utility.GetProperty(item1, "Name");
			Assert.AreEqual(item1.Name, value);
		}

		[Test]
		public void Insert_FirstItem()
		{
			ContentItem root = CreateOneItem<UtilityItem>(10, "10", null);
			int index1 = N2.Utility.Insert(item1, root, "Published");
			Assert.AreEqual(0, index1);
			Assert.AreEqual(item1, root.Children[0]);
		}

		[Test]
		public void Insert_FirstItemDesc()
		{
			ContentItem root = CreateOneItem<UtilityItem>(10, "10", null);
			int index1 = N2.Utility.Insert(item1, root, "Published DESC");
			Assert.AreEqual(0, index1);
			Assert.AreEqual(item1, root.Children[0]);
		}

		[Test]
		public void Insert_SortByPublished()
		{
			ContentItem root = CreateOneItem<UtilityItem>(10, "10", null);
			item1.Published = DateTime.Now.AddSeconds(-10);
			N2.Utility.Insert(item1, root, "Published");
			int index2 = N2.Utility.Insert(item2, root, "Published");

			Assert.AreEqual(1, index2);
			Assert.AreEqual(item1, root.Children[0]);
			Assert.AreEqual(item2, root.Children[1]);
		}

		[Test]
		public void Insert_SortByPublishedDesc()
		{
			ContentItem root = CreateOneItem<UtilityItem>(10, "10", null);
			item1.Published = DateTime.Now.AddSeconds(-10);
			N2.Utility.Insert(item1, root, "Published DESC");
			int index2 = N2.Utility.Insert(item2, root, "Published DESC");

			Assert.AreEqual(0, index2);
			Assert.AreEqual(item2, root.Children[0]);
			Assert.AreEqual(item1, root.Children[1]);
		}

		[Test]
		public void Insert_SeveralRandomItems_SortByPublishedDesc()
		{
			ContentItem root = CreateOneItem<UtilityItem>(10, "10", null);

			item1.Published = DateTime.Now.AddSeconds(-50);
			item2.Published = DateTime.Now.AddSeconds(-40);
			item3.Published = DateTime.Now.AddSeconds(-30);
			item4.Published = DateTime.Now.AddSeconds(-20);
			item5.Published = DateTime.Now.AddSeconds(-10);

			N2.Utility.Insert(item2, root, "Published DESC");
			N2.Utility.Insert(item1, root, "Published DESC");
			N2.Utility.Insert(item4, root, "Published DESC");
			N2.Utility.Insert(item5, root, "Published DESC");
			N2.Utility.Insert(item3, root, "Published DESC");

			Assert.AreEqual(item5, root.Children[0]);
			Assert.AreEqual(item4, root.Children[1]);
			Assert.AreEqual(item3, root.Children[2]);
			Assert.AreEqual(item2, root.Children[3]);
			Assert.AreEqual(item1, root.Children[4]);
		}

		[Test]
		public void Insert_ChangeParent()
		{
			N2.Utility.Insert(item1, item2, "Published");
			N2.Utility.Insert(item1, item3, "Published");
			Assert.AreEqual(0, item2.Children.Count);
			Assert.AreEqual(1, item3.Children.Count);
			Assert.AreEqual(item3, item1.Parent);
		}

		[Test]
		public void Insert_RemoveParent()
		{
			N2.Utility.Insert(item1, item2, "Published");
			N2.Utility.Insert(item1, null, "Published");
			Assert.AreEqual(0, item2.Children.Count);
			Assert.IsNull(item1.Parent);
		}

		[Test]
		public void Insert_CannotAddItemToItself()
		{
			Assert.Throws<DestinationOnOrBelowItselfException>(() => N2.Utility.Insert(item1, item1, "Published"));
		}

		[Test]
		public void Insert_CannotAddItemBelowItself()
		{
			N2.Utility.Insert(item1, item2, "Published");

			Assert.Throws<DestinationOnOrBelowItselfException>(() => N2.Utility.Insert(item2, item1, "Published"));
		}

		[Test]
		public void Insert_CanReorderItem()
		{
			ContentItem root = CreateOneItem<UtilityItem>(10, "10", null);

			item1.Published = DateTime.Now.AddSeconds(-50);
			item2.Published = DateTime.Now.AddSeconds(-40);

			N2.Utility.Insert(item1, root, "Published");
			N2.Utility.Insert(item2, root, "Published");
			N2.Utility.Insert(item1, root, "Published DESC");

			Assert.AreEqual(item2, root.Children[0]);
			Assert.AreEqual(item1, root.Children[1]);
		}

		[Test]
		public void Insert_AtIndex_NoSiblings()
		{
			N2.Utility.Insert(item1, item2, 0);
			Assert.AreEqual(item2, item1.Parent);
			Assert.AreEqual(item1, item2.Children[0]);
			Assert.AreEqual(1, item2.Children.Count);
		}

		[Test]
		public void Insert_AtIndex_Last()
		{
			item2.AddTo(item1);
			N2.Utility.Insert(item3, item1, 1);
			Assert.AreEqual(item1, item3.Parent);
			Assert.AreEqual(item2, item1.Children[0]);
			Assert.AreEqual(item3, item1.Children[1]);
			Assert.AreEqual(2, item1.Children.Count);
		}

		[Test]
		public void Insert_AtIndex_Middle()
		{
			item2.AddTo(item1);
			item3.AddTo(item1);
			N2.Utility.Insert(item4, item1, 1);
			Assert.AreEqual(item1, item3.Parent);
			Assert.AreEqual(item2, item1.Children[0]);
			Assert.AreEqual(item4, item1.Children[1]);
			Assert.AreEqual(item3, item1.Children[2]);
			Assert.AreEqual(3, item1.Children.Count);
		}

		[Test]
		public void Insert_AtIndex_MoveToPrevious()
		{
			item2.AddTo(item1);
			item3.AddTo(item1);

			N2.Utility.Insert(item3, item1, 0);

			Assert.AreEqual(item3, item1.Children[0]);
			Assert.AreEqual(item2, item1.Children[1]);
			Assert.AreEqual(2, item1.Children.Count);
		}

		[Test]
		public void Insert_AtIndex_MoveToNext()
		{
			item2.AddTo(item1);
			item3.AddTo(item1);

			N2.Utility.Insert(item2, item1, 2);

			Assert.AreEqual(item3, item1.Children[0]);
			Assert.AreEqual(item2, item1.Children[1]);
			Assert.AreEqual(2, item1.Children.Count);
		}

		[Test]
		public void Insert_AtIndex_MoveToNonLast()
		{
			item2.AddTo(item1);
			item3.AddTo(item1);
			item4.AddTo(item1);
			item5.AddTo(item1);

			N2.Utility.Insert(item2, item1, 2);

			Assert.AreEqual(item2, item1.Children[1]);
			Assert.AreEqual(4, item1.Children.Count);
		}

		[Test]
		public void Insert_AtIndex_MoveToLast()
		{
			item2.AddTo(item1);
			item3.AddTo(item1);
			item4.AddTo(item1);
			item5.AddTo(item1);

			N2.Utility.Insert(item2, item1, 4);

			Assert.AreEqual(item2, item1.Children[3]);
			Assert.AreEqual(4, item1.Children.Count);
		}

		// Utility.ExtractFirstSentences

		[Test]
		public void ExtractFirstSentences_Extracts_SingleSentence_WithinLength()
		{
			string input = "Hello world!";
			string output = N2.Utility.ExtractFirstSentences(input, 100);

			Assert.That(output, Is.EqualTo(input));
		}

		[Test]
		public void ExtractFirstSentences_Extracts_SingleSentence_WithinLength_InHtml()
		{
			string input = "<p>Hello world!</p>";
			string output = N2.Utility.ExtractFirstSentences(input, 100);

			Assert.That(output, Is.EqualTo("Hello world!"));
		}

		[Test]
		public void ExtractFirstSentences_Extracts_SingleSentence_WithinLength_InHtml_WithAttributes()
		{
			string input = "<p><a href='#'>Hello</a> world!</p>";
			string output = N2.Utility.ExtractFirstSentences(input, 100);

			Assert.That(output, Is.EqualTo("Hello world!"));
		}

		[Test]
		public void ExtractFirstSentences_Extracts_FirstSentence_WithinLength()
		{
			string input = "Hello world. Hello there!";
			string output = N2.Utility.ExtractFirstSentences(input, 20);

			Assert.That(output, Is.EqualTo("Hello world."));
		}

		[Test]
		public void ExtractFirstSentences_Extracts_TwoSentence_WithinLength()
		{
			string input = "Hello world. Hello there. Bye bye!";
			string output = N2.Utility.ExtractFirstSentences(input, 30);

			Assert.That(output, Is.EqualTo("Hello world. Hello there."));
		}

		[Test]
		public void ExtractFirstSentences_GracefullyHandles_Null()
		{
			string input = null;
			string output = N2.Utility.ExtractFirstSentences(input, 30);

			Assert.That(output, Is.EqualTo(input));
		}

		[Test]
		public void ExtractFirstSentences_GracefullyHandles_EmptyString()
		{
			string input = "";
			string output = N2.Utility.ExtractFirstSentences(input, 30);

			Assert.That(output, Is.EqualTo(input));
		}

		[Test]
		public void ExtractFirstSentences_GracefullyHandles_NoDelimiter()
		{
			string input = "Hello world";
			string output = N2.Utility.ExtractFirstSentences(input, 30);

			Assert.That(output, Is.EqualTo(input));
		}

	}
}