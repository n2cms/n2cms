using System.Xml.XPath;

namespace N2.Serialization
{
	public interface IXmlReader
	{
		void Read(XPathNavigator navigator, ContentItem item, ReadingJournal journal);
	}
}