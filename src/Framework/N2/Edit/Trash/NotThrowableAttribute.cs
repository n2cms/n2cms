using N2.Definitions;
using System;

namespace N2.Edit.Trash
{
    /// <summary>
    /// When used on an item definition this attribute prevents it from beeing 
    /// moved to trash upon deletion.
    /// </summary>
    [Obsolete("Use [Throwable(AllowInTrash.No)]")]
    public class NotThrowableAttribute : ThrowableAttribute
    {
        public NotThrowableAttribute()
            : base(AllowInTrash.No)
        {
        }
    }
}
