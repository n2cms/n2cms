using N2.Edit.FileSystem;
namespace N2.Persistence.Serialization
{
    /// <summary>
    /// A binary attachment to serialized items.
    /// </summary>
	public class Attachment
	{
		private readonly IAttachmentHandler handler;
		private readonly string url;
		private readonly byte[] fileContents = null;
		private readonly ContentItem enclosingItem;

		public Attachment(IAttachmentHandler handler, string url, ContentItem enclosingItem, byte[] fileContents)
		{
			this.handler = handler;
			this.url = url;
			this.enclosingItem = enclosingItem;
			this.fileContents = fileContents;
		}

		public string Url
		{
			get { return url; }
		}

		public ContentItem EnclosingItem
		{
			get { return enclosingItem; }
		}

		public bool HasContents
		{
			get { return FileContents != null && FileContents.Length > 0; }
        }

		public byte[] FileContents
		{
			get { return fileContents; }
		}

		public virtual void Import(IFileSystem fs)
		{
            handler.Import(fs, this);
        }
	}
}
