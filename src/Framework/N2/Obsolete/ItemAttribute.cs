using System;
using System.ComponentModel;

namespace N2
{
    [Obsolete("Changed name to DefinitionAttribute.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ItemAttribute : DefinitionAttribute
    {
        /// <summary>
        /// Initializes a new instance of ItemAttribute class.
        /// </summary>
        /// <param name="title">The title used when presenting this item type to editors.</param>
        public ItemAttribute(string title)
            : base(title)
        {
        }

        /// <summary>Initializes a new instance of ItemAttribute class.</summary>
        /// <param name="title">The title used when presenting this item type to editors.</param>
        /// <param name="name">The name/discriminator needed to map the appropriate type with content data when retrieving from persistence. When this is null the type's full name is used.</param>
        public ItemAttribute(string title, string name)
            : base(title, name)
        {
        }

        /// <summary>Initializes a new instance of ItemAttribute class.</summary>
        /// <param name="title">The title used when presenting this item type to editors.</param>
        /// <param name="name">The name/discriminator needed to map the appropriate type with content data when retrieving from persistence. When this is null the type's name is used.</param>
        /// <param name="description">The description of this item.</param>
        /// <param name="toolTip">The tool tip displayed when hovering over this item type.</param>
        /// <param name="sortOrder">The order of this type compared to other items types.</param>
        public ItemAttribute(string title, string name, string description, string toolTip, int sortOrder)
            : base(title, name, description, toolTip, sortOrder)
        {
        }
    }
}
