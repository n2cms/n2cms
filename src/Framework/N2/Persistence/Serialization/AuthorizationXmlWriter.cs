using System.Xml;
using N2.Security;

namespace N2.Persistence.Serialization
{
    public class AuthorizationXmlWriter : IXmlWriter
    {
        public virtual void Write(ContentItem item, XmlTextWriter writer)
        {
            using (new ElementWriter("authorizations", writer))
            {
                if (item.AuthorizedRoles != null)
                {
                    foreach (AuthorizedRole ar in item.AuthorizedRoles)
                    {
                        WriteRole(writer, ar);
                    }
                }
            }
        }

        protected virtual void WriteRole(XmlTextWriter writer, AuthorizedRole ar)
        {
            using (ElementWriter role = new ElementWriter("role", writer))
            {
                role.Write(ar.Role);
            }
        }
    }
}
