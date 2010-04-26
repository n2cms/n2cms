using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Serialization
{
    /// <summary>
    /// Hints for the export service.
    /// </summary>
    [Flags]
    public enum ExportOptions
    {
        /// <summary>Default/Export everything.</summary>
        Default = 0,
        /// <summary>Don't export details which have no definition.</summary>
        OnlyDefinedDetails = 1,
        /// <summary>Don't export attachments.</summary>
        ExcludeAttachments = 2
    }
}
