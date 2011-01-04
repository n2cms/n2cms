using System.Xml;

namespace N2.Persistence.Serialization
{
	public class ChildXmlWriter : IXmlWriter
	{
		public virtual void Write(ContentItem item, XmlTextWriter writer)
		{
			using (new ElementWriter("children", writer))
			{
				foreach (ContentItem child in item.Children)
				{
					WriteChild(writer, child);
				}
			}
		}

		protected virtual void WriteChild(XmlTextWriter writer, ContentItem child)
		{
			using (ElementWriter childElement = new ElementWriter("child", writer))
			{
				childElement.WriteAttribute("id", child.ID);
			}
		}
	}
}
