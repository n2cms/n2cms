using System;
using System.Collections.Generic;
using System.Text;
using N2.Definitions;

namespace N2.Edit.Trash
{
    /// <summary>
    /// When used on an item definition this attribute prevents it from beeing 
    /// moved to trash upon deletion.
    /// </summary>
    public class NotThrowableAttribute : ThrowableAttribute
    {
		public NotThrowableAttribute()
			: base(AllowInTrash.No)
		{
		}
    }
}
