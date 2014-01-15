using System;

namespace N2.Persistence.Serialization
{
    [Flags]
    public enum ImportOption
    {
        /// <summary>All items and attachments.</summary>
        All = AllItems | Attachments,
        /// <summary>All items except attachments</summary>
        AllItems = Root | Children,
        /// <summary>The root node.</summary>
        Root = 1,
        /// <summary>All items except the root node.</summary>
        Children = 2,
        /// <summary>Import attachment overwriting any existing files.</summary>
        Attachments = 4
    }
}
