using System;
using System.Collections.Generic;
using System.Web.UI;

namespace N2.Web.UI.WebControls
{
    /// <summary>
    /// Classes implementing this interface can serve as item editors.
    /// </summary>
    public interface IItemEditor : IItemContainer
    {
        /// <summary>Gets or sets versioning mode when saving items.</summary>
        ItemEditorVersioningMode VersioningMode { get; set; }
        
        /// <summary>Gets or sets the zone name to use for items saved through this editor.</summary>
        string ZoneName { get; set; }

        /// <summary>Map of editor names and controls added to this item editor.</summary>
        IDictionary<string, Control> AddedEditors { get; }

        event EventHandler<ItemEventArgs> Saved;
		event Action<object, Edit.Workflow.CommandContext> CreatingContext;
    }
}
