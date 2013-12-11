using N2.Edit.FileSystem;
namespace N2.Persistence.Serialization
{
    /// <summary>
    /// A binary attachment to serialized items.
    /// </summary>
    public class Attachment
    {
        private readonly IAttachmentHandler _handler;
        private readonly string _url;
        private readonly byte[] _fileContents;
        private readonly ContentItem _enclosingItem;

        public Attachment(IAttachmentHandler handler, string url, ContentItem enclosingItem, byte[] fileContents)
        {
            _handler = handler;
            _url = url;
            _enclosingItem = enclosingItem;
            _fileContents = fileContents;
        }

        public string Url
        {
            get { return _url; }
        }

        public ContentItem EnclosingItem
        {
            get { return _enclosingItem; }
        }

        public bool HasContents
        {
            get { return FileContents != null && FileContents.Length > 0; }
        }

        public byte[] FileContents
        {
            get { return _fileContents; }
        }

        public virtual void Import(IFileSystem fs)
        {
            _handler.Import(fs, this);
        }
    }
}
