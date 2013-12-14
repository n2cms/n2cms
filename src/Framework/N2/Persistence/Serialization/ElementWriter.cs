using System;
using System.IO;
using System.Xml;

namespace N2.Persistence.Serialization
{
    public class ElementWriter : IDisposable
    {
        public XmlTextWriter Writer { get; private set; }

        public ElementWriter(string elementName, XmlTextWriter writer)
        {
            this.Writer = writer;
            writer.WriteStartElement(elementName);
        }

        public ElementWriter(string elementName, TextWriter writer)
            : this(elementName, new XmlTextWriter(writer))
        {
        }

        #region WriteAttribute

        public void WriteAttribute(string attributeName, int value)
        {
            WriteAttribute(attributeName, value.ToString());
        }

        public void WriteAttribute(string attributeName, bool value)
        {
            WriteAttribute(attributeName, value.ToString());
        }

        public void WriteAttribute(string attributeName, DateTime value)
        {
            WriteAttribute(attributeName, SerializationUtility.ToUniversalString(value));
        }

        public void WriteAttribute(string attributeName, DateTime? value)
        {
            WriteAttribute(attributeName, value.HasValue ? SerializationUtility.ToUniversalString(value.Value) : string.Empty);
        }

        public void WriteAttribute(string attributeName, string value)
        {
            Writer.WriteAttributeString(attributeName, value);
        }

        public void WriteAttribute(string prefix, string localName, string ns, string value)
        {
            Writer.WriteAttributeString(prefix, localName, ns, value);
        }

        #endregion

        public void Dispose()
        {
            Writer.WriteEndElement();
        }

        public void Write(string value)
        {
            Writer.WriteString(value);
        }

        public void WriteCData(string value)
        {
            Writer.WriteCData(value);
        }
    }
}
