namespace N2.Persistence
{
    /// <summary>
    /// Instructs the system on where to store a property.
    /// </summary>
    public enum PropertyPersistenceLocation
    {
        /// <summary>The property is untouched. The getter and setter implementations can choose to store as details.</summary>
        Ignore,

        /// <summary>Auto-implemented virtual properties will be intercepted and stored as details. Other properties are ignored.</summary>
        Detail,

        /// <summary>Auto-implemented virtual properties will be intercepted and stored as detail collection values. Other properties are ignored.</summary>
        DetailCollection,

        /// <summary>The property is stored in the item table. This option requires the database schema to be updated.</summary>
        Column,

        /// <summary>The property is stored as a child to the content item. Only <see cref="ContentItem"/> is allowed for this persistence location.</summary>
        Child,

        /// <summary>The attribute implements <see cref="IValueAccessor"/> and provides get/set functionality.</summary>
        ValueAccessor
    }
}
