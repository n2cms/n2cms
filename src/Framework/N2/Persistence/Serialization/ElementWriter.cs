using System;
using System.IO;
using System.Xml;

namespace N2.Persistence.Serialization
{
	public class ElementWriter : IDisposable
	{
		private readonly XmlTextWriter writer;

		public ElementWriter(string elementName, XmlTextWriter writer)
		{
			this.writer = writer;
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
            WriteAttribute(attributeName, ToUniversalString(value));
		}

	    public void WriteAttribute(string attributeName, DateTime? value)
		{
            WriteAttribute(attributeName, value.HasValue ? ToUniversalString(value.Value) : string.Empty);
        }

        internal static string ToUniversalString(DateTime? value)
        {
			if (!value.HasValue)
				return "";
            return value.Value.ToUniversalTime().ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

		public void WriteAttribute(string attributeName, string value)
		{
			writer.WriteAttributeString(attributeName, value);
		}

		public void WriteAttribute(string prefix, string localName, string ns, string value)
		{
			writer.WriteAttributeString(prefix, localName, ns, value);
		}

		#endregion

		public void Dispose()
		{
			writer.WriteEndElement();
		}

		public void Write(string value)
		{
			writer.WriteString(value);
		}

		public void WriteCData(string value)
		{
			writer.WriteCData(value);
		}
	}
}
