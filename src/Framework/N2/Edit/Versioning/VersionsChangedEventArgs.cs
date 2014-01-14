using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit.Versioning
{
    public class VersionsChangedEventArgs : EventArgs
    {
        public ContentVersion Version { get; set; }
    }
}
