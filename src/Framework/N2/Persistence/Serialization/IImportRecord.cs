using System;
using System.Collections.Generic;

namespace N2.Persistence.Serialization
{
    public interface IImportRecord
    {
        IList<ContentItem> ReadItems { get; }
        ContentItem RootItem { get; }
        IList<Attachment> Attachments { get; }
        IList<Attachment> FailedAttachments { get; }
		IList<Tuple<ContentItem, Exception>> FailedContentItems { get; } 
		IList<Exception> Errors { get; }
        IList<UnresolvedLink> UnresolvedLinks { get; }
    }
}
