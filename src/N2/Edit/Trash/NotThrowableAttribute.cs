using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Edit.Trash
{
    /// <summary>
    /// When used on an item definition this attribute prevents it from beeing 
    /// moved to trash upon deletion.
    /// </summary>
    public class NotThrowableAttribute : Attribute
    {
    }
}
