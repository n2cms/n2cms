using System.Xml;

namespace N2.Serialization
{
	public interface IXmlWriter
	{
		void Write(ContentItem item, XmlTextWriter writer);
	}
}