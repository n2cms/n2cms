using System.Xml;
using N2.Edit.Versioning;

namespace N2.Persistence.Serialization
{
    public class ChildXmlWriter : IXmlWriter
    {
        private ExportOptions options;

        public ChildXmlWriter(ExportOptions options)
        {
            this.options = options;
        }

        public virtual void Write(ContentItem item, XmlTextWriter writer)
        {
            using (new ElementWriter("children", writer))
            {
                foreach (ContentItem child in ItemXmlWriter.GetChildren(item, options))
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
                childElement.WriteAttribute("name", child.Name);
                childElement.WriteAttribute("versionIndex", child.VersionIndex);

                if (child.VersionOf.HasValue)
                    childElement.WriteAttribute("versionOf", child.VersionOf.Value.ID);
                if (child.GetVersionKey() != null)
                    childElement.WriteAttribute("versionKey", child.GetVersionKey());
            }
        }
    }
}
