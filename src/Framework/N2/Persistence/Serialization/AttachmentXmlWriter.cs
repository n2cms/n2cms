using System.Collections.Generic;
using System.Xml;
using N2.Definitions;
using N2.Edit.FileSystem;

namespace N2.Persistence.Serialization
{
	public class AttachmentXmlWriter : IXmlWriter
	{
		private readonly AttributeExplorer explorer = new AttributeExplorer();
		private IFileSystem fs;

		public AttachmentXmlWriter(IFileSystem fs)
		{
			this.fs = fs;
		}

		public void Write(ContentItem item, XmlTextWriter writer)
		{
			IList<IAttachmentHandler> attachments = explorer.Find<IAttachmentHandler>(item.GetContentType());
			if(attachments.Count > 0)
			{
				using(new ElementWriter("attachments", writer))
				{
					foreach(IAttachmentHandler attachment in attachments)
					{
						using (ElementWriter attachmentElement = new ElementWriter("attachment", writer))
						{
							attachmentElement.WriteAttribute("name", attachment.Name);
							attachment.Write(fs, item, writer);
						}
					}
				}
			}
		}
	}
}
