using System.IO;

namespace N2.Details
{
    /// <summary>
    /// Represents a displayable attribute that can write it's value directly 
    /// to a text writer without need of page context.
    /// </summary>
    public interface IWritingDisplayable : IDisplayable // If you remove this inheritance consider ordering of the displayable renderers
    {
        /// <summary>Writes a detail value.</summary>
        /// <param name="item">The item containing the value.</param>
        /// <param name="detailName">The name of the property or detail.</param>
        /// <param name="writer">The writer to write the value to, unless the value is null.</param>
        void Write(ContentItem item, string detailName, TextWriter writer);
    }
}
