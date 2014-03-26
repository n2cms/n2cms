using System;

namespace N2.Persistence.Serialization
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
        ExcludeAttachments = 2,
        /// <summary>Don't export children that are not parts.</summary>
        ExcludeParts = 4,
        /// <summary>Don't export children that are not parts.</summary>
        ExcludePages = 8,
        /// <summary>Don't export children at all.</summary>
        ExcludeChildren = ExcludeParts | ExcludePages
    }
}
