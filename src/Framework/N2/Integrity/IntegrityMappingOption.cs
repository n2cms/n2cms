namespace N2.Integrity
{
    /// <summary>
    /// Flag used to control how types defined by the
    /// <see cref="IntegrityMappingAttribute"/> are added to item definitions.
    /// </summary>
    public enum IntegrityMappingOption
    {
        /// <summary>Add defined types to existing ones.</summary>
        AddToExising,
        /// <summary>Add defined types to existing ones and remove existing types not fullfilling defined types.</summary>
        RemoveOthers
    }
}
