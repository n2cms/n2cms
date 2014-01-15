using System.Xml.XPath;

namespace N2.Persistence.Serialization
{
    public interface IXmlReader
    {
        void Read(XPathNavigator navigator, ContentItem item, ReadingJournal journal);
    }
}
