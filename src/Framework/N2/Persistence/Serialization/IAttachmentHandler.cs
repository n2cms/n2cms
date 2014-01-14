using System;
using System.Xml;
using System.Xml.XPath;
using N2.Edit.FileSystem;

namespace N2.Persistence.Serialization
{
    public interface IAttachmentHandler : Definitions.IUniquelyNamed, IComparable<IAttachmentHandler>
    {
        /// <summary>Writes an attachment to the export stream.</summary>
        void Write(IFileSystem fs, ContentItem item, XmlTextWriter writer);

        /// <summary>Reads an attachment from import stream.</summary>
        Attachment Read(XPathNavigator navigator, ContentItem item);

        /// <summary>Imports an attachment into the host system.</summary>
        /// <param name="attachment">The attachment to import.</param>
        void Import(IFileSystem fs, Attachment attachment);
    }
}
