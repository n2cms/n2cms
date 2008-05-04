using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.UI;
using N2.Definitions;
using N2.Web.UI.WebControls;
using N2.Collections;
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

		/// <summary>Gets the url for the navigation frame.</summary>
		/// <param name="selectedItem">The currently selected item.</param>
		/// <returns>An url.</returns>
		string GetNavigationUrl(INode selectedItem);

		/// <summary>Gets the url for the preview frame.</summary>
		/// <param name="selectedItem">The currently selected item.</param>
		/// <returns>An url.</returns>
		string GetPreviewUrl(INode selectedItem);

		/// <summary>Gets the text editor css url.</summary>
		/// <returns>An url to a style sheet.</returns>
		string GetEditorCssUrl();

		/// <summary>Gets the url to edit page creating new items.</summary>
		/// <param name="selected">The selected item.</param>
		/// <param name="definition">The type of item to edit.</param>
		/// <param name="zoneName">The zone to add the item to.</param>
		/// <param name="position">The position relative to the selected item to add the item.</param>
		/// <returns>The url to the edit page.</returns>
		string GetEditNewPageUrl(ContentItem selected, ItemDefinition definition, string zoneName, CreationPosition position);

		/// <summary>Gets the url to the edit page where to edit an existing item.</summary>
		/// <param name="item">The item to edit.</param>
		/// <returns>The url to the edit page</returns>
		string GetEditExistingItemUrl(ContentItem item);

		/// <summary>Gets the url to the upload folder.</summary>
		/// <returns>An url to the upload root.</returns>
		string GetUploadFolderUrl();

		///// <summary>Gets toolbar plugins for the edit mode.</summary>
		///// <param name="user">The user whose permissions will filter return plugins.</param>
		///// <returns>A list of toolbar plpugins.</returns>
		//IList<ToolbarPluginAttribute> GetToolbarPlugIns(IPrincipal user);

		///// <summary>Gets navigation plugins for the edit mode.</summary>
		///// <param name="user">The user whose permissions will filter return plugins.</param>
		///// <returns>A list of navigation plpugins.</returns>
		//IList<NavigationPluginAttribute> GetNavigationPlugIns(IPrincipal user);

		/// <summary>Gets edit mode plugins found in the environment sorted and filtered by the given user.</summary>
		/// <typeparam name="T">The type of plugin to get.</typeparam>
		/// <param name="user">The user that should be authorized for the plugin.</param>
		/// <returns>An enumeration of plugins.</returns>
		IEnumerable<T> GetPlugins<T>(IPrincipal user)
			where T : AdministrativePluginAttribute;

		/// <summary>Adds defined editors and containers to a control.</summary>
		/// <param name="itemType">The type of content item whose editors to add.</param>
		/// <param name="editorContainer">The container onto which add the editors.</param>
		/// <param name="user">The user whose permissions to use when adding editors.</param>
		/// <returns>A list of added editors.</returns>
		IDictionary<string, Control> AddEditors(Type itemType, Control editorContainer, IPrincipal user);

		/// <summary>Sets initial editor values.</summary>
		/// <param name="addedEditors">Previously added editor controls.</param>
		/// <param name="item">The content item to use for update.</param>
		/// <param name="user">The current user.</param>
		void UpdateEditors(ContentItem item, IDictionary<string, Control> addedEditors, IPrincipal user);

		/// <summary>Updates the item with the values from the editors.</summary>
		/// <param name="item">The item to update.</param>
		/// <param name="addedEditors">The previously added editors.</param>
		/// <param name="user">The user for filtering updatable editors.</param>
		bool UpdateItem(ContentItem item, IDictionary<string, Control> addedEditors, IPrincipal user);

		/// <summary>Saves an item using values from the supplied item editor.</summary>
		/// <param name="itemEditor">The item editor containing the values to update with.</param>
		/// <param name="user">The user that is performing the saving.</param>
		ContentItem Save(IItemEditor itemEditor, IPrincipal user);

		/// <summary>Gets the url to the edit interface.</summary>
		/// <param name="selectedItem">The item to select in edit mode.</param>
		/// <returns>The url to the edit interface.</returns>
		string GetEditModeUrl(ContentItem selectedItem);

		/// <summary>Gets the url to the select type of item to create.</summary>
		/// <param name="selectedItem">The currently selected item.</param>
		/// <returns>The url to the select new item to create page.</returns>
		string GetSelectNewItemUrl(ContentItem selectedItem);

		/// <summary>Gets the url to the select type of item to create.</summary>
		/// <param name="selectedItem">The currently selected item.</param>
		/// <param name="zoneName">The zone to select.</param>
		/// <returns>The url to the select new item to create page.</returns>
		string GetSelectNewItemUrl(ContentItem selectedItem, string zoneName);

		/// <summary>Gets the url to the delete item page.</summary>
		/// <param name="selectedItem">The currently selected item.</param>
		/// <returns>The url to the delete page.</returns>
		string GetDeleteUrl(ContentItem selectedItem);

		ItemFilter GetEditorFilter(IPrincipal user);
	}
}
