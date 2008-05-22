using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using N2.Definitions;

namespace N2.Serialization
{
	public class AttachmentXmlWriter : IXmlWriter
	{
		private readonly AttributeExplorer<IAttachmentHandler> explorer;

		public AttachmentXmlWriter(AttributeExplorer<IAttachmentHandler> explorer)
		{
			this.explorer = explorer;
		}
		public void Write(ContentItem item, XmlTextWriter writer)
		{
			IList<IAttachmentHandler> attachments = explorer.Find(item.GetType());
			if(attachments.Count > 0)
			{
				using(new ElementWriter("attachments", writer))
				{
					foreach(IAttachmentHandler attachment in attachments)
					{
						using (ElementWriter attachmentElement = new ElementWriter("attachment", writer))
						{
							attachmentElement.WriteAttribute("name", attachment.Name);
							attachment.Write(item, writer);
						}
					}
				}
			}
		}
	}
}
