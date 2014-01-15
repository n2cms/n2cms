using System;
using System.Web.UI;

namespace N2.Definitions
{
    /// <summary>
    /// Attributes implementing this interface defines editors of a content 
    /// item. The interface defines methods to add editor controls to a web 
    /// form and updating items with their values.
    /// </summary>
    public interface IEditable : IContainable, IComparable<IEditable>
    {
        /// <summary>Gets or sets the label used for presentation.</summary>
        string Title { get; set;}

        /// <summary>Updates the object with the values from the editor.</summary>
        /// <param name="item">The object to update.</param>
        /// <param name="editor">The editor contorl whose values to update the object with.</param>
        /// <returns>True if the item was changed (and needs to be saved).</returns>
        bool UpdateItem(ContentItem item, Control editor);

        /// <summary>Updates the editor with the values from the object.</summary>
        /// <param name="item">The object that contains values to assign to the editor.</param>
        /// <param name="editor">The editor to load with a value.</param>
        void UpdateEditor(ContentItem item, Control editor);
    }
}
