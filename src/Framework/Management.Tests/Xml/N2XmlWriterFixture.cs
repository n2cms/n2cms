//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Xml;
//using NUnit.Framework;

//namespace N2.Tests.Xml
//{
//    [TestFixture]
//    public class N2XmlWriterFixture : Persistence.DatabasePreparingBase
//    {
//        private N2.Xml.N2XmlWriter writer = null;
//        private N2.Xml.N2XmlReader reader = null;

//        [SetUp]
//        public override void SetUp()
//        {
//            base.SetUp();
//            writer = new N2.Xml.N2XmlWriter(this.engine);
//            reader = new N2.Xml.N2XmlReader(this.engine);
//        }

//        [Test]
//        public void WriteOneNode()
//        {
//            ContentItem root = CreateRoot("root", "root item");
//            string xml = writer.GetXml(root).ToString();
//            Assert.IsNotNull(xml);
//            Assert.Greater(xml.IndexOf("<item "), 0);
//            engine.Persister.Delete(root);
//        }

//        [Test]
//        public void WriteAndReadOneNodeWithDetails()
//        {
//            ContentItem root = CreateRoot("root", "root item");
//            root["bool"] = true;
//            root["int"] = 123;
//            root["double"] = 123.123;
//            root["date"] = DateTime.Parse("2007-02-04 22:51:49");
//            root["string"] = "Just a string";
//            root["link"] = root;
//            root["object"] = new string[] { "hello", "hiho", "hoho" };

//            ContentItem reReadItem = ToXmlAndBack(root);

//            DatesAreEqual(root.Created, reReadItem.Created);
//            DatesAreEqual(root.Expires, reReadItem.Expires);
//            Assert.AreEqual(root.Name, reReadItem.Name);
//            DatesAreEqual(root.Published, reReadItem.Published);
//            Assert.AreEqual(root.SavedBy, reReadItem.SavedBy);
//            Assert.AreEqual(root.SortOrder, reReadItem.SortOrder);
//            Assert.AreEqual(root.Title, reReadItem.Title);
//            DatesAreEqual(root.Updated, reReadItem.Updated);
//            Assert.AreEqual(root.Visible, reReadItem.Visible);
//            Assert.AreEqual(root.ZoneName, reReadItem.ZoneName);

//            Assert.AreEqual(root["bool"], reReadItem["bool"]);
//            Assert.AreEqual(root["int"], reReadItem["int"]);
//            Assert.AreEqual(root["double"], reReadItem["double"]);
//            Assert.AreEqual(root["date"], reReadItem["date"]);
//            Assert.AreEqual(root["string"], reReadItem["string"]);
//            Assert.IsNull(reReadItem["link"]); // TODO fix this
//            ArraysAreEqual((string[])root["object"], (string[])reReadItem["object"]);

//            engine.Persister.Delete(root);
//        }

//        [Test]
//        public void WriteAndReadNodeWithCollection()
//        {
//            ContentItem root = CreateRoot("root", "root item");
//            root.GetDetailCollection("collection1", true).Add("just a value");

//            ContentItem item = ToXmlAndBack(root);
//            Assert.AreEqual(root.GetDetailCollection("collection1", false).Count, item.GetDetailCollection("collection1", false).Count);
//            Assert.AreEqual(root.GetDetailCollection("collection1", false)[0], item.GetDetailCollection("collection1", false)[0]);

//            engine.Persister.Delete(root);
//        }

//        [Test]
//        public void WriteAndReadAllKindsOfObjectsInCollection()
//        {
//            ContentItem root = CreateRoot("root", "root item");
//            Details.DetailCollection collection = root.GetDetailCollection("collection1", true);
//            collection.Add(true);//bool
//            collection.Add(123);
//            collection.Add(123.123);
//            collection.Add(DateTime.Parse("2007-02-04 22:51:49"));
//            collection.Add("Just a string");
//            collection.Add(root);
//            collection.Add(new string[] { "hello", "hiho", "hoho" });

//            ContentItem item = ToXmlAndBack(root);
//            // TODO fix link details
//            Assert.AreEqual(root.GetDetailCollection("collection1", false).Count - 1, item.GetDetailCollection("collection1", false).Count);
//            Assert.AreEqual(root.GetDetailCollection("collection1", false)[0], item.GetDetailCollection("collection1", false)[0]);

//            engine.Persister.Delete(root);
//        }

//        [Test]
//        public void WriteAndReadNodeWithChildren()
//        {
//            ContentItem root = CreateRoot("root", "root item");
//            ContentItem item1 = CreateAndSaveItem("item1", "item one", root);
//            ContentItem item2 = CreateAndSaveItem("item2", "item two", root);
//            ContentItem item3 = CreateAndSaveItem("item3", "item three", root);

//            ContentItem reReadItem = ToXmlAndBack(root);
//            Assert.AreEqual(root.Name, reReadItem.Name);
//            Assert.AreEqual(root.Children.Count, reReadItem.Children.Count);
//            Assert.AreEqual(item1.Name, reReadItem.GetChild("item1").Name);
//            Assert.AreEqual(item2.Name, reReadItem.GetChild("item2").Name);
//            Assert.AreEqual(item3.Name, reReadItem.GetChild("item3").Name);
//        }

//        #region Utility Methods
//        private ContentItem ToXmlAndBack(ContentItem item)
//        {
//            string xml = writer.GetXml(item).ToString();
//            return reader.ReadXml(xml);
//        }

//        private static void DatesAreEqual(DateTime? expected, DateTime? actual)
//        {
//            Assert.AreEqual(expected.ToString(), actual.ToString());
//        }

//        private void ArraysAreEqual(object[] expected, object[] actual)
//        {
//            for (int i = 0; i < expected.Length; i++)
//                Assert.AreEqual(expected[i], actual[i]);
//        }
//        #endregion
//    }
//}
