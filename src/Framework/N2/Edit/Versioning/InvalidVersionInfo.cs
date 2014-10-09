using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit.Versioning
{
    public sealed class InvalidVersionInfo : VersionInfo
    {
	    public InvalidVersionInfo()
	    {
		    Title = "Invalid version";
			State = ContentState.None;
		    SavedBy = string.Empty;
	    }

		//public new int ID
		//{
		//	get { return 0; }
		//	set { throw new InvalidOperationException("Can't set the ID of the invalid version"); }
		//}
		//public new string Title
		//{
		//	get { return "Invalid version"; }
		//	set { throw new InvalidOperationException("Can't set title of the invalid version"); }
		//}
		//public new ContentState State
		//{
		//	get { return ContentState.None; }
		//	set { throw new InvalidOperationException(); }
		//}
		//public new string IconUrl {
		//	get { return null; }
		//	set { throw new InvalidOperationException(); }
		//}
		//public new DateTime? Published
		//{
		//	get { return null; }
		//	set { throw new InvalidOperationException(); }
		//}
		//public new DateTime? FuturePublish
		//{
		//	get { return null; }
		//	set { throw new InvalidOperationException(); }
		//}
		//public new DateTime? Expires
		//{
		//	get { return null; }
		//	set { throw new InvalidOperationException(); }
		//}
		//public new int VersionIndex
		//{
		//	get { return 0; }
		//	set { throw new InvalidOperationException(); }
		//}
		//public new int PartsCount
		//{
		//	get { return 0; }
		//	set { throw new InvalidOperationException(); }
		//}
		//public new string SavedBy
		//{
		//	get { return null; }
		//	set { throw new InvalidOperationException(); }
		//}

	    public new ContentItem Content
	    {
		    get { return null; }
	    }

	    public new Func<ContentItem> ContentFactory { get; set; }
	    public Exception InnerException { get; set; }
    }
}
