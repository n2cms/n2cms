namespace N2.Collections
{
    /// <summary>
    /// An item that can be placed in a zone.
    /// </summary>
    public interface IPlaceable
    {
        /// <summary>The name of the zone where the item has been placed.</summary>
        string ZoneName { get; }
    }
}
