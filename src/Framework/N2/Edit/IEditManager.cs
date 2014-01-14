using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.UI;
using N2.Collections;
using N2.Definitions;
using N2.Web.UI.WebControls;

namespace N2.Edit
{
    /// <summary>
    /// Classes implementing this interface can be used to interact with the 
    /// edit mode functionality.
    /// </summary>
    public interface IEditManager
    {
        /// <summary>Gets or sets wether a version is saved when updating items.</summary>
        bool EnableVersioning { get; set; }

        /// <summary>Gets folders paths on the server where users are allowed to upload content through the interface.</summary>
        IList<string> UploadFolders { get; }

        string EditTheme { get; }

        /// <summary>Gets edit mode plugins found in the environment sorted and filtered by the given user.</summary>
        /// <typeparam name="T">The type of plugin to get.</typeparam>
        /// <param name="user">The user that should be authorized for the plugin.</param>
        /// <returns>An enumeration of plugins.</returns>
        IEnumerable<T> GetPlugins<T>(IPrincipal user)
            where T : AdministrativePluginAttribute;

        /// <summary>Adds defined editors and containers to a control.</summary>
        /// <param name="definition">The definition containing editor information.</param>
        /// <param name="item">The content item whose editors to add.</param>
        /// <param name="container">The container onto which add the editors.</param>
        /// <param name="user">The user whose permissions to use when adding editors.</param>
        /// <returns>A list of added editors.</returns>
        IDictionary<string, Control> AddEditors(ItemDefinition definition, ContentItem item, Control container, IPrincipal user);

        /// <summary>Adds defined editors and containers to a control.</summary>
        /// <param name="definition">The definition containing editor information.</param>
        /// <param name="item">The content item whose editors to add.</param>
        /// <param name="container">The container onto which add the editors.</param>
        /// <param name="user">The user whose permissions to use when adding editors.</param>
        /// <param name="editableNameFilter">Only add editors with these names.</param>
        /// <param name="containerTypeFilter">Only add editors within this container type.</param>
        /// <returns>A list of added editors.</returns>
        IDictionary<string, Control> AddEditors(ItemDefinition definition, ContentItem item, Control container, IPrincipal user, Type containerTypeFilter, IEnumerable<string> editableNameFilter);

        /// <summary>Sets initial editor values.</summary>
        /// <param name="addedEditors">Previously added editor controls.</param>
        /// <param name="definitions"></param>
        /// <param name="item">The content item to use for update.</param>
        /// <param name="user">The current user.</param>
        void UpdateEditors(ItemDefinition definitions, ContentItem item, IDictionary<string, Control> addedEditors, IPrincipal user);

        /// <summary>Updates the item with the values from the editors.</summary>
        /// <param name="definitions"></param>
        /// <param name="item">The item to update.</param>
        /// <param name="addedEditors">The previously added editors.</param>
        /// <param name="user">The user for filtering updatable editors.</param>
        /// <returns>Details that were updated.</returns>
        string[] UpdateItem(ItemDefinition definitions, ContentItem item, IDictionary<string, Control> addedEditors, IPrincipal user);

        /// <summary>Saves an item using values from the supplied item editor.</summary>
        /// <param name="item">The item to update.</param>
        /// <param name="addedEditors">The editors to update the item with.</param>
        /// <param name="versioningMode">How to treat the item beeing saved in respect to versioning.</param>
        /// <param name="user">The user that is performing the saving.</param>
        [Obsolete("Use CommandDispatcher")]
        ContentItem Save(ContentItem item, IDictionary<string, Control> addedEditors, ItemEditorVersioningMode versioningMode, IPrincipal user);

        /// <summary>Gets the filter to be applied to items displayed in edit mode.</summary>
        /// <param name="user">The user for whom to apply the filter.</param>
        /// <returns>A filter.</returns>
        ItemFilter GetEditorFilter(IPrincipal user);

        /// <summary>Use EditUrlManager instead.</summary>
        [Obsolete("Use EditUrlManager")]
        string GetDeleteUrl(ContentItem selectedItem);

        /// <summary>Use EditUrlManager instead.</summary>
        [Obsolete("Use EditUrlManager")]
        string GetEditExistingItemUrl(ContentItem item);

        /// <summary>Use EditUrlManager instead.</summary>
        [Obsolete("Use EditUrlManager")]
        string GetEditInterfaceUrl();

        /// <summary>Use EditUrlManager instead.</summary>
        [Obsolete("Use EditUrlManager")]
        string GetEditInterfaceUrl(ContentItem selectedItem);

        /// <summary>Use EditUrlManager instead.</summary>
        [Obsolete("Use EditUrlManager")]
        string GetEditNewPageUrl(ContentItem selected, ItemDefinition definition, string zoneName, CreationPosition position);

        /// <summary>Use EditUrlManager instead.</summary>
        [Obsolete("Use EditUrlManager")]
        string GetManagementInterfaceUrl();

        /// <summary>Use EditUrlManager instead.</summary>
        [Obsolete("Use EditUrlManager")]
        string GetNavigationUrl(INode selectedItem);

        /// <summary>Use EditUrlManager instead.</summary>
        [Obsolete("Use EditUrlManager")]
        string GetPreviewUrl(INode selectedItem);

        /// <summary>Use EditUrlManager instead.</summary>
        [Obsolete("Use EditUrlManager")]
        string GetSelectNewItemUrl(ContentItem selectedItem);

        /// <summary>Use EditUrlManager instead.</summary>
        [Obsolete("Use EditUrlManager")]
        string GetSelectNewItemUrl(ContentItem selectedItem, string zoneName);
    }
}
