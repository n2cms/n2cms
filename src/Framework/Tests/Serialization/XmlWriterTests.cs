using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using N2.Details;
using N2.Persistence.Serialization;
using N2.Security;
using N2.Tests.Serialization.Items;
using NUnit.Framework;
using System.Web;
using N2.Tests.Fakes;

namespace N2.Tests.Serialization
{
    [TestFixture]
    public class XmlWriterTests : XmlSerializationTestsBase
    {
        [Test]
        public void WriteSingleItem()
        {
            XmlableItem item = CreateOneItem<XmlableItem>(1, "one", null);

            XPathNavigator xpn = WriteToStreamAndNavigate(item);

            Assert.AreEqual(1, xpn.Select("//item[@id='1']").Count);
            Assert.AreEqual(1, xpn.Select("//item[@name='one']").Count);
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

            XPathNavigator xpn = WriteToStreamAndNavigate(item);

            string theMillenium = new DateTime(2000, 1, 1).ToUniversalTime().ToString(System.Globalization.CultureInfo.InvariantCulture);

            Assert.AreEqual(1, xpn.Select("//item[@id='2']").Count);
            Assert.AreEqual(1, xpn.Select("//item[@parent='1']").Count);
            Assert.AreEqual(1, xpn.Select("//item[@name='two']").Count);
            Assert.AreEqual(1, xpn.Select("//item[@title='xml item']").Count);
            Assert.AreEqual(1, xpn.Select("//item[@zoneName='danger']").Count);
            Assert.AreEqual(1, xpn.Select("//item[@created='" + theMillenium + "']").Count);
            Assert.AreEqual(1, xpn.Select("//item[@updated='" + theMillenium + "']").Count);
            Assert.AreEqual(1, xpn.Select("//item[@published='" + theMillenium + "']").Count);
            Assert.AreEqual(1, xpn.Select("//item[@expires='" + theMillenium + "']").Count);
            Assert.AreEqual(1, xpn.Select("//item[@sortOrder='2']").Count);
            Assert.AreEqual(1, xpn.Select("//item[@visible='False']").Count);
            Assert.AreEqual(1, xpn.Select("//item[@savedBy='cristian']").Count);
            Assert.AreEqual(1, xpn.Select("//item[@typeName='" + string.Format("{0},{1}", typeof(XmlableItem).AssemblyQualifiedName.Split(',')) + "']").Count);
            Assert.AreEqual(1, xpn.Select("//item[@discriminator='"+ typeof(XmlableItem).Name +"']").Count);
        }

        [Test]
        public void WriteItem_WithBooleanDetail()
        {
            XmlableItem item = CreateOneItem<XmlableItem>(1, "one", null);
            item["Exists"] = true;

            XPathNavigator xpn = WriteToStreamAndNavigate(item);

            XPathNodeIterator nodes = xpn.Select("//item/details/detail[@name='Exists']");
            nodes.MoveNext();

            Assert.AreEqual(1, nodes.Count);
            Assert.AreEqual("True", nodes.Current.Value);
        }

        [Test]
        public void WriteItem_WithIntegerDetail()
        {
            XmlableItem item = CreateOneItem<XmlableItem>(1, "one", null);
            item["Age"] = 28;

            XPathNavigator xpn = WriteToStreamAndNavigate(item);

            XPathNodeIterator nodes = xpn.Select("//item/details/detail[@name='Age']");
            nodes.MoveNext();

            Assert.AreEqual(1, nodes.Count);
            Assert.AreEqual(28.ToString(), nodes.Current.Value);
        }

        [Test]
        public void WriteItem_WithDoubleDetail()
        {
            XmlableItem item = CreateOneItem<XmlableItem>(1, "one", null);
            item["Weight"] = 73.4;

            XPathNavigator xpn = WriteToStreamAndNavigate(item);

            XPathNodeIterator nodes = xpn.Select("//item/details/detail[@name='Weight']");
            nodes.MoveNext();

            Assert.AreEqual(1, nodes.Count);
            Assert.AreEqual(73.4.ToString(), nodes.Current.Value);
        }

        [Test]
        public void WriteItem_WithStringDetail()
        {
            XmlableItem item = CreateOneItem<XmlableItem>(1, "one", null);
            item["FavouriteColor"] = "black";

            XPathNavigator xpn = WriteToStreamAndNavigate(item);

            XPathNodeIterator nodes = xpn.Select("//item/details/detail[@name='FavouriteColor']");
            nodes.MoveNext();

            Assert.AreEqual(1, nodes.Count);
            Assert.AreEqual("black", nodes.Current.Value);
        }

        [Test]
        public void WriteItem_WithStringDetail_ComplexString()
        {
            XmlableItem item = CreateOneItem<XmlableItem>(1, "one", null);
            string alphabet = "abcdefghijklmnopqrstuvwxyzåäöABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖ[]{}<|>!\"#¤%&/()=?,.-_:;¨'^*`´\\}][{@£$§½";
            item["Alphabet"] = alphabet;

            XPathNavigator xpn = WriteToStreamAndNavigate(item);

            XPathNodeIterator nodes = xpn.Select("//item/details/detail[@name='Alphabet']");
            nodes.MoveNext();

            Assert.AreEqual(1, nodes.Count);
            Assert.AreEqual(HttpUtility.HtmlEncode(alphabet), nodes.Current.Value);
        }

        [Test]
        public void WriteItem_WithObjectDetail()
        {
            XmlableItem item = CreateOneItem<XmlableItem>(1, "one", null);
            object[] array = new object[] { 1, "hello", 1.2, true };
            item["MixedFeelings"] = array;

            XPathNavigator xpn = WriteToStreamAndNavigate(item);

            XPathNodeIterator nodes = xpn.Select("//item/details/detail[@name='MixedFeelings']");
            nodes.MoveNext();

            Assert.AreEqual(1, nodes.Count);

            object[] fromString = Deserialize(nodes) as object[];

            for (int i = 0; i < array.Length; i++)
            {
                Assert.AreEqual(array[i], fromString[i]);
            }
        }

        [Test]
        public void WriteItem_WithLinkDetail()
        {
            XmlableItem item = CreateOneItem<XmlableItem>(1, "one", null);
            XmlableItem other = CreateOneItem<XmlableItem>(2, "other", null);
            item["Target"] = other;

            XPathNavigator xpn = WriteToStreamAndNavigate(item);

            XPathNodeIterator nodes = xpn.Select("//item/details/detail[@name='Target']");
            nodes.MoveNext();

            Assert.AreEqual(1, nodes.Count);
            Assert.AreEqual("2", nodes.Current.Value);
        }

        [Test]
        public void WriteItem_WithNoAuthorizedRoles()
        {
            XmlableItem item = CreateOneItem<XmlableItem>(1, "one", null);

            XPathNavigator xpn = WriteToStreamAndNavigate(item);

            XPathNodeIterator nodes = xpn.Select("//item/authorized/role");

            Assert.AreEqual(0, nodes.Count);
        }

        [Test]
        public void WriteItem_WithAuthorizedRole()
        {
            XmlableItem item = CreateOneItem<XmlableItem>(1, "one", null);
            item.AuthorizedRoles.Add(new AuthorizedRole(item, "Administrator"));
            XPathNavigator xpn = WriteToStreamAndNavigate(item);

            XPathNodeIterator nodes = xpn.Select("//item/authorizations/role");
            nodes.MoveNext();

            Assert.AreEqual(1, nodes.Count);
            Assert.AreEqual("Administrator", nodes.Current.Value);
        }

        [TestCase(new bool[] { true, false, true }, typeof(bool))]
        [TestCase(new int[] { 1, 2, 3, 5, 8, 13 }, typeof(int))]
        [TestCase(new double[] { 1.2, 2.3, 3.4, 5.5, 8.6, 13.7 }, typeof(double))]
        [TestCase(new string[] { "one", "two", "three", "four" }, typeof(string))]
        public void WriteItem_WithDetailCollection(Array array, Type type)
        {
            XmlableItem item = CreateOneItem<XmlableItem>(1, "one", null);
            DetailCollection dc = item.GetDetailCollection("Collected", true);
            foreach (object  o in array)
            {
                dc.Add(o);
            }

            XPathNavigator xpn = WriteToStreamAndNavigate(item);

            XPathNodeIterator nodes = xpn.Select("//item/detailCollections/collection[@name='Collected']/detail");

            Assert.AreEqual(array.Length, nodes.Count);
            while(nodes.MoveNext())
            {
                EnumerableAssert.Contains(array, Convert.ChangeType(nodes.Current.Value, type));
            }
        }

        [Test]
        public void WriteItem_WithLinkCollection()
        {
            XmlableItem item = CreateOneItem<XmlableItem>(1, "one", null);
            XmlableItem two = CreateOneItem<XmlableItem>(2, "two", null);
            XmlableItem three = CreateOneItem<XmlableItem>(3, "three", null);
            XmlableItem four = CreateOneItem<XmlableItem>(4, "four", null);
            DetailCollection dc = item.GetDetailCollection("Related", true);
            dc.Add(two);
            dc.Add(three);
            dc.Add(four);

            XPathNavigator xpn = WriteToStreamAndNavigate(item);

            XPathNodeIterator nodes = xpn.Select("//item/detailCollections/collection[@name='Related']/detail");

            Assert.AreEqual(3, nodes.Count);
            nodes.MoveNext();
            Assert.AreEqual("2", nodes.Current.Value);
            nodes.MoveNext();
            Assert.AreEqual("3", nodes.Current.Value);
            nodes.MoveNext();
            Assert.AreEqual("4", nodes.Current.Value);
        }
        

        [Test]
        public void WriteItem_WithChildren_ChildrenAreReferencedByItem()
        {
            XmlableItem item = CreateOneItem<XmlableItem>(1, "one", null);
            CreateOneItem<XmlableItem>(2, "two", item);
            CreateOneItem<XmlableItem>(3, "three", item);
            CreateOneItem<XmlableItem>(4, "four", item);

            XPathNavigator xpn = WriteToStreamAndNavigate(item);

            XPathNodeIterator nodes = xpn.Select("//item/children/child");

            Assert.AreEqual(3, nodes.Count);
            nodes.MoveNext();
            Assert.AreEqual("2", nodes.Current.GetAttribute("id", string.Empty));
            nodes.MoveNext();
            Assert.AreEqual("3", nodes.Current.GetAttribute("id", string.Empty));
            nodes.MoveNext();
            Assert.AreEqual("4", nodes.Current.GetAttribute("id", string.Empty));
        }

        [Test]
        public void WriteItem_WithChildren_ChildrenAreAppendedToStream()
        {
            XmlableItem item = CreateOneItem<XmlableItem>(1, "one", null);
            XmlableItem two = CreateOneItem<XmlableItem>(2, "two", item);
            XmlableItem three = CreateOneItem<XmlableItem>(3, "three", item);
            XmlableItem four = CreateOneItem<XmlableItem>(4, "four", item);

            XPathNavigator xpn = WriteToStreamAndNavigate(item);

            XPathNodeIterator nodes = xpn.Select("/n2/item");

            Assert.AreEqual(4, nodes.Count);
            nodes.MoveNext();
            Assert.AreEqual("1", nodes.Current.GetAttribute("id", string.Empty));
            nodes.MoveNext();
            Assert.AreEqual("2", nodes.Current.GetAttribute("id", string.Empty));
            nodes.MoveNext();
            Assert.AreEqual("3", nodes.Current.GetAttribute("id", string.Empty));
            nodes.MoveNext();
            Assert.AreEqual("4", nodes.Current.GetAttribute("id", string.Empty));
        }

        [Test]
        public void WriteItem_WithChildren_GrandChildrenAreAppendedToStream()
        {
            XmlableItem item = CreateOneItem<XmlableItem>(1, "one", null);
            XmlableItem two = CreateOneItem<XmlableItem>(2, "two", item);
            XmlableItem three = CreateOneItem<XmlableItem>(3, "three", two);
            XmlableItem four = CreateOneItem<XmlableItem>(4, "four", three);

            XPathNavigator xpn = WriteToStreamAndNavigate(item);

            XPathNodeIterator nodes = xpn.Select("/n2/item");

            Assert.AreEqual(4, nodes.Count);

            nodes.MoveNext();
            Assert.AreEqual("1", nodes.Current.GetAttribute("id", string.Empty));
            XPathNodeIterator children = nodes.Current.Select("children/child");
            children.MoveNext();
            Assert.AreEqual("2", children.Current.GetAttribute("id", string.Empty));
            
            nodes.MoveNext();
            Assert.AreEqual("2", nodes.Current.GetAttribute("id", string.Empty));
            children = nodes.Current.Select("children/child");
            children.MoveNext();
            Assert.AreEqual(1, children.Count);
            Assert.AreEqual("3", children.Current.GetAttribute("id", string.Empty));
            
            nodes.MoveNext();
            Assert.AreEqual("3", nodes.Current.GetAttribute("id", string.Empty));
            children = nodes.Current.Select("children/child");
            children.MoveNext();
            Assert.AreEqual(1, children.Count);
            Assert.AreEqual("4", children.Current.GetAttribute("id", string.Empty));
            
            nodes.MoveNext();
            Assert.AreEqual("4", nodes.Current.GetAttribute("id", string.Empty));
            children = nodes.Current.Select("children/child");
            children.MoveNext();
            Assert.AreEqual(0, children.Count);
        }

        private static object Deserialize(XPathNodeIterator nodes)
        {
            byte[] buffer = Convert.FromBase64String(nodes.Current.Value);
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(new MemoryStream(buffer));
        }

        [Test]
        public void AttachmentXmlWriter_CanWrite()
        {
            XmlableItem item = CreateOneItem<XmlableItem>(1, "item", null);
            item.ImageUrl = "da image";
            AttachmentXmlWriter axw = new AttachmentXmlWriter(new FakeMemoryFileSystem());

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            XmlTextWriter xtw = new XmlTextWriter(sw);

            axw.Write(item, xtw);

            Assert.That(sb.ToString(), Is.StringContaining("DA IMAGE"));
        }
    }
}
