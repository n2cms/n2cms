using System;
using System.Xml;
using System.Xml.XPath;
using N2.Persistence.Serialization;
using NUnit.Framework;
using N2.Edit.FileSystem;

namespace N2.Tests.Serialization.Items
{
    public class FakeAttachment : Attribute, IAttachmentHandler
    {
        private string name;

        public void Write(IFileSystem fs, ContentItem item, XmlTextWriter writer)
        {
            writer.WriteStartElement("fake");
            string x = item[Name] as string;
            writer.WriteAttributeString("blah", x);
            writer.WriteCData(x.ToUpper());
            writer.WriteEndElement();
            item["wasWritten"] = true;
        }

        public Attachment Read(XPathNavigator navigator, ContentItem item)
        {
            string x = navigator.GetAttribute("blah", string.Empty);
            if (!string.IsNullOrEmpty(x))
            {
                Assert.AreEqual(x.ToUpper(), navigator.Value);
                item["wasRead"] = true;
                return new Attachment(this, x, item, new byte[1]);
            }
            return null;
        }

        public void Import(IFileSystem fs, Attachment attachment)
        {
            Assert.IsTrue((bool) attachment.EnclosingItem["wasRead"]);
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int CompareTo(IAttachmentHandler other)
        {
            return Name.CompareTo(other.Name);
        }
    }
}
