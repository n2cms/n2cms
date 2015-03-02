using System;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Xml.XPath;
using N2.Details;
using N2.Persistence.Serialization;
using N2.Security;
using N2.Tests.Serialization.Items;
using NUnit.Framework;
using System.Collections;
using System.Web;
using N2.Tests.Fakes;

namespace N2.Tests.Serialization
{
	[TestFixture]
	public class XmlReaderTests : XmlSerializationTestsBase
	{
		[Test]
		public void CanReReadBasicProperties()
		{
			XmlableItem item = CreateOneItem<XmlableItem>(1, "the item", null);
			XPathNavigator sr = SerializeAndReadOutput(item);

			IItemXmlReader reader = CreateReader();
			ContentItem readItem = reader.Read(sr).RootItem;

			Assert.AreNotSame(item, readItem);
			Assert.AreEqual(typeof(XmlableItem), readItem.GetContentType());
			Assert.AreEqual(1, readItem.ID);
			Assert.AreEqual("the item", readItem.Name);
		}

		[Test]
		public void CanReReadProperties()
		{
			XmlableItem parent = CreateOneItem<XmlableItem>(1, "parent", null);
			XmlableItem item = CreateOneItem<XmlableItem>(2, "two", parent);
			item.Title = "xml item";
			item.ZoneName = "danger";
			item.Created = new DateTime(2000, 1, 1);
			item.Updated = new DateTime(2001, 1, 1);
			item.Published = new DateTime(2002, 1, 1);
			item.Expires = new DateTime(2003, 1, 1);
			item.SortOrder = 2;
			item.Visible = false;
			item.SavedBy = "cristian";

			ContentItem readItem = Mangle(item);

			Assert.IsNull(readItem.Parent);
			Assert.AreEqual("xml item", readItem.Title);
			Assert.AreEqual("danger", readItem.ZoneName);
			Assert.AreEqual(new DateTime(2000, 1, 1), readItem.Created);
			Assert.AreEqual(new DateTime(2001, 1, 1), readItem.Updated);
			Assert.AreEqual(new DateTime(2002, 1, 1), readItem.Published);
			Assert.AreEqual(new DateTime(2003, 1, 1), readItem.Expires);
			Assert.AreEqual(2, readItem.SortOrder);
			Assert.IsFalse(readItem.Visible);
			Assert.AreEqual("cristian", readItem.SavedBy);
		}

		[Test]
		public void CanRead_ItemWithBooleanDetail()
		{
			XmlableItem item = CreateOneItem<XmlableItem>(1, "item", null);
			item["Nasty"] = true;

			ContentItem readItem = Mangle(item);

			Assert.IsTrue((bool)readItem["Nasty"]);
		}

		[Test]
		public void CanRead_ItemWithIntegerDetail()
		{
			XmlableItem item = CreateOneItem<XmlableItem>(1, "item", null);
			item["Score"] = 4;

			ContentItem readItem = Mangle(item);

			Assert.AreEqual(4, (int)readItem["Score"]);
		}

		[Test]
		public void CanRead_ItemWithDoubleDetail()
		{
			XmlableItem item = CreateOneItem<XmlableItem>(1, "item", null);
			item["Temperature"] = 22.4;

			ContentItem readItem = Mangle(item);

			Assert.AreEqual(22.4, (double)readItem["Temperature"]);
		}

		[Test]
		public void CanRead_ItemWithStringDetail()
		{
			XmlableItem item = CreateOneItem<XmlableItem>(1, "item", null);
			item["Greeting"] = "Hello World!";

			ContentItem readItem = Mangle(item);

			Assert.AreEqual("Hello World!", readItem["Greeting"]);
		}

		[TestCase("When you are courting a nice girl an hour seems like a second. When you sit on a red-hot cinder a second seems like an hour. That's relativity.")]
		[TestCase("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ []{}<|>!\"#¤%&/()=?,.-_:;¨'^*`´\\}][{@£$§½")]
		[TestCase("êüâÌŸ¼¾¿¡åäöÅÄÖ")]
		[TestCase("京仅尽径惊琎痉紧经警谨鲸")]
		[TestCase("ĈĠĜĝĤĥĴĵŜŝŬŭ")]
		[TestCase("Яговорюпо-русски.")]
		[TestCase("Αυτουοιθανατονμητσομαι")]
		public void CanRead_ItemWithUnicodeStringDetail(string text)
		{
			XmlableItem item = CreateOneItem<XmlableItem>(1, "item", null);
			item["MysticText"] = text;

			ContentItem readItem = Mangle(item);

			Assert.AreEqual(text, readItem["MysticText"]);
		}

		[Test]
		public void CanRead_ItemWithObjectDetail()
		{
			XmlableItem item = CreateOneItem<XmlableItem>(1, "item", null);
			item["Pair"] = new Pair("First", 2);

			ContentItem readItem = Mangle(item);

			Pair p = (Pair)readItem["Pair"];
			Assert.AreEqual("First", p.First);
			Assert.AreEqual(2, p.Second);
		}

		//[Test]
		//public void ItemReadEventIsInvoked()
		//{
		//    XmlableItem item = CreateOneItem<XmlableItem>(1, "item", null);
		//    item["Pair"] = new Pair("First", 2);

		//    XPathNavigator sr = SerializeAndReadOutput(item);
		//    ItemXmlReader reader = CreateReader();
		//    reader.ItemRead += reader_ItemRead;
		//    ContentItem readItem = reader.Read(sr);

		//    Assert.IsNotNull(lastReadItem);
		//    Assert.AreEqual(readItem, lastReadItem);
		//}

		//private ContentItem lastReadItem = null;
		//void reader_ItemRead(object sender, N2.Persistence.ItemEventArgs e)
		//{
		//    lastReadItem = e.AffectedItem;
		//}

		[Test]
		public void ExportedChildrenAreAddedToReturnedItem()
		{
			XmlableItem item = CreateOneItem<XmlableItem>(1, "item", null);
			XmlableItem child1 = CreateOneItem<XmlableItem>(2, "child1", item);
			XmlableItem child2 = CreateOneItem<XmlableItem>(3, "child2", item);
			XmlableItem child3 = CreateOneItem<XmlableItem>(4, "child3", item);

			ContentItem readItem = Mangle(item);

			Assert.AreEqual(3, readItem.Children.Count);
			EnumerableAssert.Contains(readItem.Children, child1);
			EnumerableAssert.Contains(readItem.Children, child2);
			EnumerableAssert.Contains(readItem.Children, child3);
		}

		[Test]
		public void CanRead_ItemWithLinkDetail()
		{
			XmlableItem item = CreateOneItem<XmlableItem>(1, "item", null);
			XmlableItem child1 = CreateOneItem<XmlableItem>(2, "child1", item);
			XmlableItem child2 = CreateOneItem<XmlableItem>(3, "child2", item);
			XmlableItem child3 = CreateOneItem<XmlableItem>(4, "child3", item);
			child1["Prev"] = child3;
			child2["Prev"] = child1;
			child3["Prev"] = child2;
			child1["Next"] = child2;
			child2["Next"] = child3;
			child3["Next"] = child1;
			ContentItem readItem = Mangle(item);

			Assert.AreEqual(3, readItem.Children.Count);
			Assert.AreEqual(child3, readItem.Children[0]["Prev"]);
			Assert.AreEqual(child2, readItem.Children[0]["Next"]);
			Assert.AreEqual(child1, readItem.Children[1]["Prev"]);
			Assert.AreEqual(child3, readItem.Children[1]["Next"]);
			Assert.AreEqual(child2, readItem.Children[2]["Prev"]);
			Assert.AreEqual(child1, readItem.Children[2]["Next"]);
		}

		[Test]
		public void ExportedGrandChildrenAreAddedToReturnedItem()
		{
			XmlableItem item = CreateOneItem<XmlableItem>(1, "item", null);
			XmlableItem child1 = CreateOneItem<XmlableItem>(2, "child1", item);
			XmlableItem child2 = CreateOneItem<XmlableItem>(3, "child2", child1);
			XmlableItem child3 = CreateOneItem<XmlableItem>(4, "child3", child2);

			ContentItem readItem = Mangle(item);

			Assert.AreEqual(1, readItem.Children.Count);
			Assert.AreEqual(child1, readItem.Children[0]);
			Assert.AreEqual(child2, readItem.Children[0].Children[0]);
			Assert.AreEqual(child3, readItem.Children[0].Children[0].Children[0]);
		}

		[TestCase(new bool[] { true, false, true })]
		[TestCase(new int[] { 1, 2, 3, 5, 8, 13, 21 })]
		[TestCase(new double[] { 1.0, 2.718, 3.1415, 1234567890.12 })]
		public void CanRead_DetailCollection(IEnumerable values)
		{
			XmlableItem item = CreateOneItem<XmlableItem>(1, "item", null);
			DetailCollection dc = item.GetDetailCollection("Details", true);
			foreach (object detail in values)
			{
				dc.Add(detail);
			}

			ContentItem readItem = Mangle(item);

			DetailCollection readCollection = readItem.GetDetailCollection("Details", false);
			Assert.IsNotNull(readCollection);
			foreach (object detail in values)
			{
				if (detail is string)
					EnumerableAssert.Contains(readCollection, (string)detail);
				else
					EnumerableAssert.Contains(readCollection, detail);
			}
		}

		[Test]
		public void CanRead_WithCollectionOfStrings()
		{
			// using testcase attribute doesn't compile
			CanRead_DetailCollection(new string[] { "hello", "shiney", "ring", "O'Hara", "1 < 2 and 3 > 1" });
		}

		[Test]
		public void CanRead_WithCollectionOfLinks()
		{
			XmlableItem item = CreateOneItem<XmlableItem>(1, "item", null);
			CreateOneItem<XmlableItem>(2, "child1", item);
			CreateOneItem<XmlableItem>(3, "child2", item);
			CreateOneItem<XmlableItem>(4, "child3", item);
			CreateOneItem<XmlableItem>(5, "child4", item);
			CreateOneItem<XmlableItem>(6, "child5", item);

			foreach (ContentItem child in item.Children)
			{
				DetailCollection dc = child.GetDetailCollection("Siblings", true);
				foreach (ContentItem sibling in item.Children)
				{
					if (sibling != child)
						dc.Add(sibling);
				}
			}

			ContentItem readItem = Mangle(item);

			foreach (ContentItem child in readItem.Children)
			{
				Assert.IsNotNull(child.GetDetailCollection("Siblings", false));
				Assert.AreEqual(4, child.GetDetailCollection("Siblings", false).Count);
			}
		}

		[Test]
		public void CanRead_ItemWithAuthorizedRoles()
		{
			XmlableItem item = CreateOneItem<XmlableItem>(1, "item", null);
			item.AuthorizedRoles.Add(new AuthorizedRole(item, "Administrators"));

			ContentItem readItem = Mangle(item);

			Assert.AreEqual(1, readItem.AuthorizedRoles.Count);
			Assert.AreEqual("Administrators", readItem.AuthorizedRoles[0].Role);
		}

		[Test]
		public void CanReReadProperties_WithStrangeCharacters()
		{
			string weirdo = "<[{zuuuuagh}]> & co?!=!";
			XmlableItem item = CreateOneItem<XmlableItem>(1, "<[{zuuuuagh}]> co!=!", null);
			item.Title = weirdo;
			item.ZoneName = weirdo;
			item.SavedBy = weirdo;

			XPathNavigator sr = SerializeAndReadOutput(item);
			IItemXmlReader reader = CreateReader();
			ContentItem readItem = reader.Read(sr).RootItem;

			Assert.AreNotSame(item, readItem);
			Assert.AreEqual(typeof(XmlableItem), readItem.GetContentType());
			Assert.AreEqual(1, readItem.ID);
			Assert.AreEqual("<[{zuuuuagh}]> co!=!", readItem.Name);
			Assert.AreEqual(weirdo, readItem.Title);
			Assert.AreEqual(weirdo, readItem.ZoneName);
			Assert.AreEqual(weirdo, readItem.SavedBy);

		}

		[Test]
		public void CanDeserializeAttachment()
		{
			XmlableItem item = CreateOneItem<XmlableItem>(1, "item", null);
			item.ImageUrl = "/my/special/image.jpg";

			ContentItem readItem = Mangle(item);

			Assert.IsTrue((bool)readItem["wasRead"]);
		}

		[Test]
		public void CanIgnore_MissingDiscriminator()
		{
			var root = CreateOneItem<XmlableItem>(1, "root", null);
			var child = CreateOneItem<XmlableItem2>(2, "child", root);

			var navigator = SerializeAndReadOutput(root);

			ItemXmlReader reader = new ItemXmlReader(TestSupport.SetupDefinitions(typeof(XmlableItem)), activator);
			reader.IgnoreMissingTypes = true;
			var readRoot = reader.Read(navigator);

			Assert.That(readRoot.RootItem.Name, Is.EqualTo("root"));
			Assert.That(readRoot.RootItem.Children.Count, Is.EqualTo(0));
		}

		private ContentItem Mangle(ContentItem item)
		{
			XPathNavigator navigator = SerializeAndReadOutput(item);

			IItemXmlReader reader = CreateReader();
			return reader.Read(navigator).RootItem;
		}

		private XPathNavigator SerializeAndReadOutput(ContentItem item)
		{
			StringBuilder sb = new StringBuilder();
			StringWriter sw = new StringWriter(sb);

			CreateExporter().Export(item, ExportOptions.Default, sw);

			StringReader sr = new StringReader(sb.ToString());
			XPathNavigator navigator = new XPathDocument(sr).CreateNavigator();
			navigator.MoveToFirstChild();
			return navigator;
		}
	}
}
