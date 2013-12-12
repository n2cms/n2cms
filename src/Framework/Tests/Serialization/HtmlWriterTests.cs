using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using N2.Tests.Serialization.Items;
using NUnit.Framework;

namespace N2.Tests.Serialization
{
    [TestFixture]
    internal class HtmlWriterTests : HtmlSerializationTestsBase
    {
        [Test]
        public void WriteSingleItem()
        {
            XmlableItem item = CreateOneItem<XmlableItem>(1, "one", null);
            
            Console.WriteLine(WriteToStreamAndReturn(item));
            //Assert.AreEqual(1, xpn.Select("//item[@id='1']").Count);
            //Assert.AreEqual(1, xpn.Select("//item[@name='one']").Count);
        }

        [Test]
        public void WriteItem_WithMultipleAttributes()
        {
            XmlableItem parent = CreateOneItem<XmlableItem>(1, "parent", null);

            XmlableItem item = CreateOneItem<XmlableItem>(2, "two", parent);
            item.Title = "xml item";
            item.ZoneName = "danger";
            item.Created = new DateTime(2000, 1, 1);
            item.Updated = new DateTime(2000, 1, 1);
            item.Published = new DateTime(2000, 1, 1);
            item.Expires = new DateTime(2000, 1, 1);
            item.SortOrder = 2;
            item.Visible = false;
            item.SavedBy = "cristian";
            item.IsPage = false;

            Console.WriteLine(WriteToStreamAndReturn(parent));
        }

        [Test]
        public void WriteItem_WithBooleanDetail()
        {
            XmlableItem item = CreateOneItem<XmlableItem>(1, "one", null);
            item["Exists"] = true;

            Console.WriteLine(WriteToStreamAndReturn(item));
            //XPathNodeIterator nodes = xpn.Select("//item/details/detail[@name='Exists']");
            //nodes.MoveNext();
            //Assert.AreEqual(1, nodes.Count);
            //Assert.AreEqual("True", nodes.Current.Value);
        }

        [Test]
        public void WriteItem_WithIntegerDetail()
        {
            XmlableItem item = CreateOneItem<XmlableItem>(1, "one", null);
            item["Age"] = 28;

            Console.WriteLine(WriteToStreamAndReturn(item));
            //XPathNodeIterator nodes = xpn.Select("//item/details/detail[@name='Age']");
            //nodes.MoveNext();
            //Assert.AreEqual(1, nodes.Count);
            //Assert.AreEqual(28.ToString(), nodes.Current.Value);
        }

    }
}
