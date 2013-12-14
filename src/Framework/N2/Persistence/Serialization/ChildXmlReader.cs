using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using N2.Edit.Versioning;

namespace N2.Persistence.Serialization
{
    public class ChildXmlReader : XmlReader, IXmlReader
    {
        public void Read(System.Xml.XPath.XPathNavigator navigator, ContentItem item, ReadingJournal journal)
        {
            foreach (XPathNavigator childElement in EnumerateChildren(navigator))
            {
                var attributes = GetAttributes(childElement);
                int id;
                if (attributes.ContainsKey("id") && int.TryParse(attributes["id"], out id) && id != 0)
                {
                    Handle(item, journal, id);
                }
                if (attributes.ContainsKey("versionOf") && int.TryParse(attributes["versionOf"], out id) && id != 0)
                {
                    Handle(item, journal, id);
                }
                else if (attributes.ContainsKey("versionKey"))
                {
                    Handle(item, journal, attributes["versionKey"]);
                }
            }
        }

        private void Handle(ContentItem item, ReadingJournal journal, string versionKey)
        {
            var child = journal.Find(versionKey);
            if (child != null)
                child.AddTo(item);
            else
                journal.Register(versionKey, (ci) => ci.AddTo(item), isChild: true);
        }

        private static void Handle(ContentItem item, ReadingJournal journal, int id)
        {
            var child = journal.Find(id);
            if (child != null)
                child.AddTo(item);
            else
                journal.Register(id, (ci) => ci.AddTo(item), isChild: true);
        }
    }
}
