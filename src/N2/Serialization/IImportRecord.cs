using System;
using System.Collections.Generic;

namespace N2.Serialization
{
	public interface IImportRecord
	{
		IList<ContentItem> ReadItems { get; }
		ContentItem RootItem { get; }
        IList<Attachment> Attachments { get; }
        IList<Attachment> FailedAttachments { get; }
		IList<Exception> Errors { get; }
    }
}