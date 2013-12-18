using System.Collections.Generic;
using System.Xml.XPath;
using N2.Definitions;

namespace N2.Persistence.Serialization
{
    public class AttachmentXmlReader : XmlReader, IXmlReader
    {
        private readonly AttributeExplorer _explorer = new AttributeExplorer();
        
        public void Read(XPathNavigator navigator, ContentItem item, ReadingJournal journal)
        {
            IDictionary<string, IAttachmentHandler> attachments = _explorer.Map<IAttachmentHandler>(item.GetContentType());
            
            foreach(XPathNavigator attachmentElement in EnumerateChildren(navigator))
            {
                string name = attachmentElement.GetAttribute("name", string.Empty);
                if(attachments.ContainsKey(name))
                {
                    XPathNavigator attachmentContents = navigator.CreateNavigator();
                    attachmentContents.MoveToFirstChild();
                    Attachment a = attachments[name].Read(attachmentContents, item);
                    if(a != null)
                        journal.Report(a);
                }
            }
        }
    }
}
