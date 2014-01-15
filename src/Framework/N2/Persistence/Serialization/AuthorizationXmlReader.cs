using System.Xml.XPath;
using N2.Security;

namespace N2.Persistence.Serialization
{
    public class AuthorizationXmlReader : XmlReader, IXmlReader
    {
        public void Read(XPathNavigator navigator, ContentItem item, ReadingJournal journal)
        {
            foreach (XPathNavigator authorizationElement in EnumerateChildren(navigator))
            {
                string role = authorizationElement.Value;
                item.AuthorizedRoles.Add(new AuthorizedRole(item, role));
            }
        }
    }
}
