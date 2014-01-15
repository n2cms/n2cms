using System;
using System.Collections.Generic;
using System.Linq;
using N2.Collections;
using N2.Definitions;
using N2.Integrity;
using NUnit.Framework;
using N2.Edit;
using Shouldly;
using System.Globalization;
using System.Threading;

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
        public void GetFileSizeString_Test()
        {
            var bak = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            N2.Utility.GetFileSizeString(1000 * 1048576, false).ShouldBe("1,000.0 MiB");
            N2.Utility.GetFileSizeString(1048576, false).ShouldBe("1.0 MiB");
            N2.Utility.GetFileSizeString(1024, false).ShouldBe("1,024 bytes");
            N2.Utility.GetFileSizeString(4 * 1024, false).ShouldBe("4.0 KiB");
            N2.Utility.GetFileSizeString(1000000000, true).ShouldBe("1,000.0 MB");
            N2.Utility.GetFileSizeString(1000000, true).ShouldBe("1.0 MB");
            N2.Utility.GetFileSizeString(1024, true).ShouldBe("1,024 bytes");
            N2.Utility.GetFileSizeString(4 * 1000, true).ShouldBe("4.0 KB");
            N2.Utility.GetFileSizeString(1048576, true).ShouldBe("1.0 MB");
            N2.Utility.GetFileSizeString(0, true).ShouldBe("0 bytes");
            N2.Utility.GetFileSizeString(0, false).ShouldBe("0 bytes");

            Thread.CurrentThread.CurrentCulture = bak;
        }

        [Test]
        public void UpdateSortOrder_SetsUniqueSortOrder_ForEachItem()
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
        public void UpdateSortOrder_DoesntChange_AlreadyOrderedItems()
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
        public void UpdateSortOrder_CanFixOrder_OnLastItem()
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

        [Test]
        public void UpdateSortOrder_CanFixOrder_OnFirstItem_EqualToNext()
        {
            item1.SortOrder = 0;
            item2.SortOrder = 0;
            item3.SortOrder = 1;
            item4.SortOrder = 2;
            item5.SortOrder = 3;

            IEnumerable<ContentItem> changedItems = N2.Utility.UpdateSortOrder(items);

            Assert.That(changedItems.Count(), Is.EqualTo(1));
            Assert.That(item1.SortOrder, Is.LessThan(item2.SortOrder));
            Assert.That(item2.SortOrder, Is.LessThan(item3.SortOrder));
        }

        [Test]
        public void UpdateSortOrder_CanFixOrder_OnFirstItem_GreaterThanNext()
        {
            item1.SortOrder = 1000;
            item2.SortOrder = 2;
            item3.SortOrder = 3;
            item4.SortOrder = 4;
            item5.SortOrder = 5;

            IEnumerable<ContentItem> changedItems = N2.Utility.UpdateSortOrder(items);

            Assert.That(changedItems.Count(), Is.EqualTo(1));
            Assert.That(item1.SortOrder, Is.LessThan(item2.SortOrder));
            Assert.That(item2.SortOrder, Is.LessThan(item3.SortOrder));
        }

        [Test]
        public void UpdateSortOrder_CanFixOrder_OnFirstItem_EqualToNext_LowerBound()
        {
            item1.SortOrder = int.MinValue;
            item2.SortOrder = int.MinValue;
            item3.SortOrder = 0;
            item4.SortOrder = 1;
            item5.SortOrder = 2;

            IEnumerable<ContentItem> changedItems = N2.Utility.UpdateSortOrder(items);

            Assert.That(item1.SortOrder, Is.LessThan(item2.SortOrder));
            Assert.That(item2.SortOrder, Is.LessThan(item3.SortOrder));
        }

        [Test]
        public void UpdateSortOrder_CanFixOrder_OnFirstItem_GreaterThanNext_LowerBound()
        {
            item1.SortOrder = 0;
            item2.SortOrder = int.MinValue;
            item3.SortOrder = 0;
            item4.SortOrder = 1;
            item5.SortOrder = 2;

            IEnumerable<ContentItem> changedItems = N2.Utility.UpdateSortOrder(items);

            Assert.That(item1.SortOrder, Is.LessThan(item2.SortOrder));
            Assert.That(item2.SortOrder, Is.LessThan(item3.SortOrder));
        }

        [Test]
        public void UpdateSortOrder_CanFixOrder_IncrementallyOn4()
        {
            var muchosElementos = new List<ContentItem>();

            muchosElementos.Add(CreateOneItem<UtilityItem>(1, "i1", null));
            N2.Utility.UpdateSortOrder(muchosElementos);
            muchosElementos.Insert(0, CreateOneItem<UtilityItem>(2, "i2", null));
            N2.Utility.UpdateSortOrder(muchosElementos);
            muchosElementos.Insert(0, CreateOneItem<UtilityItem>(3, "i3", null));
            N2.Utility.UpdateSortOrder(muchosElementos);
            muchosElementos.Insert(0, CreateOneItem<UtilityItem>(4, "i4", null));
            N2.Utility.UpdateSortOrder(muchosElementos);

            Assert.That(muchosElementos[0].SortOrder, Is.LessThan(muchosElementos[1].SortOrder));
            Assert.That(muchosElementos[1].SortOrder, Is.LessThan(muchosElementos[2].SortOrder));
            Assert.That(muchosElementos[2].SortOrder, Is.LessThan(muchosElementos[3].SortOrder));
        }

        [Test]
        public void UpdateSortOrder_CanFixOrder_IncrementallyOn1000()
        {
            var muchosElementos = new List<ContentItem>();
            for (int i = 0; i < 1000; i++)
            {
                muchosElementos.Insert(0, CreateOneItem<UtilityItem>(i, "i" + i, null));
                N2.Utility.UpdateSortOrder(muchosElementos);
            }
            for (int i = 1; i < muchosElementos.Count; i++)
            {
                Assert.That(muchosElementos[i - 1].SortOrder, Is.LessThan(muchosElementos[i].SortOrder), "At " + i);
            }
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

        [Test]
        public void EvaluateStaticProperty_GivesPropertyValue()
        {
            object value = N2.Utility.Evaluate(new SelectionUtility((ContentItem)null, (ContentItem)null), "SelectedQueryKey");
            Assert.That(value, Is.EqualTo(SelectionUtility.SelectedQueryKey));
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
        public void SetValue_OnMissingProperty_Throws()
        {
            Should.Throw<Exception>(() => N2.Utility.SetProperty(item1, "Name2", "WhooHoo"));
        }

        [Test]
        public void TrySetValue_SetsProperty()
        {
            var result = N2.Utility.TrySetProperty(item1, "Name", "WhooHoo");
            result.ShouldBe(true);
            item1.Name.ShouldBe("WhooHoo");
        }


        [Test]
        public void TrySetValue_OnMissingProperty_ReturnsFalse()
        {
            var result = N2.Utility.TrySetProperty(item1, "Name2", "WhooHoo");
            result.ShouldBe(false);
        }

        [Test]
        public void SetValue_OnNestedItem_SetsTheValue()
        {
            item2.Parent = item1;
            N2.Utility.SetProperty(item2, "Parent.Name", "WhooHoo");
            Assert.AreEqual("WhooHoo", item2.Parent.Name);
        }

        [Test]
        public void TrySetValue_OnNestedItem_SetsTheValue()
        {
            item2.Parent = item1;
            var result = N2.Utility.TrySetProperty(item2, "Parent.Name", "WhooHoo");
            result.ShouldBe(true);
            Assert.AreEqual("WhooHoo", item2.Parent.Name);
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
        public void CanGetProperty_OfNestedProperty()
        {
            item2.Parent = item1;
            object value = N2.Utility.GetProperty(item2, "Parent.Name");
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
            item1.Published = N2.Utility.CurrentTime().AddSeconds(-10);
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
            item1.Published = N2.Utility.CurrentTime().AddSeconds(-10);
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

            item1.Published = N2.Utility.CurrentTime().AddSeconds(-50);
            item2.Published = N2.Utility.CurrentTime().AddSeconds(-40);
            item3.Published = N2.Utility.CurrentTime().AddSeconds(-30);
            item4.Published = N2.Utility.CurrentTime().AddSeconds(-20);
            item5.Published = N2.Utility.CurrentTime().AddSeconds(-10);

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

            item1.Published = N2.Utility.CurrentTime().AddSeconds(-50);
            item2.Published = N2.Utility.CurrentTime().AddSeconds(-40);

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

        [Test]
        public void ExtractFirstSentences_GracefullyHandles_NoDelimiter_AndParagraphs()
        {
            string input = "<p>Test test test</p>";
            string output = N2.Utility.ExtractFirstSentences(input, 250);

            Assert.That(output, Is.EqualTo("Test test test"));
        }

        [Test]
        public void ExtractFirstSentences_GracefullyHandles_OnlyParagraphs()
        {
            string input = "<p></p>";
            string output = N2.Utility.ExtractFirstSentences(input, 250);

            Assert.That(output, Is.EqualTo(""));
        }

        [TestCase(typeof(object), 0)]
        [TestCase(typeof(ContentItem), 2)] // ContentItem(2) : INode(1) : ILink(0)
        [TestCase(typeof(UtilityItem), 3)] // UtilityItem(3) : ContentItem(2) : INode(1) : ILink(0)
        public void InheritanceDepth_Classes(Type type, int expectedDepth)
        {
            Assert.That(N2.Utility.InheritanceDepth(type), Is.EqualTo(expectedDepth));
        }

        [TestCase(typeof(ICloneable), 0)]
        [TestCase(typeof(IEditable), 3)] // IEditable(3) : IContainable(2) : IUniquelyNamed(1) : INameable(0)
        [TestCase(typeof(IContainable), 2)] // IContainable(2) : IUniquelyNamed(1) : INameable(0)
        [TestCase(typeof(IUniquelyNamed), 1)] // IUniquelyNamed(1) : INameable(0)
        [TestCase(typeof(INameable), 0)] // INameable(0)
        public void InheritanceDepth_Interfaces(Type type, int expectedDepth)
        {
            Assert.That(N2.Utility.InheritanceDepth(type), Is.EqualTo(expectedDepth));
        }

        [TestCase(typeof(object), 0)]
        [TestCase(typeof(ContentItem), 1)]
        [TestCase(typeof(UtilityItem), 2)]
        public void GetBaseTypes_Classes(Type type, int expectedDepth)
        {
            Assert.That(N2.Utility.GetBaseTypes(type).Count(), Is.EqualTo(expectedDepth));
        }
        
        [Test]
        public void GetBaseTypes_OfInterfaces_IsZero()
        {
            Assert.That(N2.Utility.GetBaseTypes(typeof(IEditable)).Count(), Is.EqualTo(0));
        }
    }
}
