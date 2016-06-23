using N2.Definitions;

namespace N2.Edit
{
    public interface IEditUrlManager
    {
        /// <summary>Gets the url for the navigation frame.</summary>
        string EditTreeUrl { get; }

        /// <summary>Gets the url for the media browser frame.</summary>
        string MediaBrowserUrl { get; }

        /// <summary>Gets the url for the navigation frame.</summary>
        /// <param name="selectedItem">The currently selected item.</param>
        /// <returns>An url.</returns>
        string GetNavigationUrl(ContentItem selectedItem);

        /// <summary>Gets the url for the preview frame.</summary>
        /// <param name="selectedItem">The currently selected item.</param>
        /// <returns>An url.</returns>
        string GetPreviewUrl(ContentItem selectedItem);

        /// <summary>Gets the url to edit page creating new items.</summary>
        /// <param name="selected">The selected item.</param>
        /// <param name="definition">The type of item to edit.</param>
        /// <param name="zoneName">The zone to add the item to.</param>
        /// <param name="position">The position relative to the selected item to add the item.</param>
        /// <returns>The url to the edit page.</returns>
        string GetEditNewPageUrl(ContentItem selected, ItemDefinition definition, string zoneName = null, CreationPosition position = CreationPosition.Below);

        /// <summary>Gets the url to the edit page where to edit an existing item.</summary>
        /// <param name="item">The item to edit.</param>
        /// <returns>The url to the edit page</returns>
        string GetEditExistingItemUrl(ContentItem item, string returnUrl = null);

        /// <summary>Gets the url to the edit interface with a certain item selected.</summary>
        /// <param name="selectedItem">The item to select in edit mode.</param>
        /// <returns>The url to the edit interface.</returns>
        string GetEditInterfaceUrl(ContentItem selectedItem);

        /// <summary>Gets the url to the edit interface.</summary>
        /// <returns>The url to the edit interface.</returns>
        string GetEditInterfaceUrl(ViewPreference preference = ViewPreference.Published);

        /// <summary>Gets the url to the management interface.</summary>
        /// <returns>The url to the edit interface.</returns>
        string GetManagementInterfaceUrl();

        /// <summary>Gets the url to the given resource underneath the management interface.</summary>
        /// <returns>The url to the edit interface.</returns>
        string ResolveResourceUrl(string resourceUrl);

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
    }
}
