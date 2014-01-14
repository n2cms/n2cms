using System.Collections.Generic;
using System.Xml;
using N2.Definitions;
using N2.Edit.FileSystem;

namespace N2.Persistence.Serialization
{
    public class AttachmentXmlWriter : IXmlWriter
    {
        private readonly AttributeExplorer _explorer = new AttributeExplorer();
        private readonly IFileSystem _fs;

        public AttachmentXmlWriter(IFileSystem fs)
        {
            _fs = fs;
        }

        public void Write(ContentItem item, XmlTextWriter writer)
        {
            IList<IAttachmentHandler> attachments = _explorer.Find<IAttachmentHandler>(item.GetContentType());
            if(attachments.Count > 0)
            {
                using(new ElementWriter("attachments", writer))
                {
                    foreach(IAttachmentHandler attachment in attachments)
                    {
                        using (ElementWriter attachmentElement = new ElementWriter("attachment", writer))
                        {
                            attachmentElement.WriteAttribute("name", attachment.Name);
                            attachment.Write(_fs, item, writer);
                        }
                    }
                }
            }
        }
    }
}
