using System.Xml;

namespace N2.Persistence.Serialization
{
    /// <summary>
    /// Classes implementing this interface are respoinsible of writing various 
    /// elements of a content item to the output stream.
    /// </summary>
    public interface IXmlWriter
    {
        void Write(ContentItem item, XmlTextWriter writer);
    }
}
