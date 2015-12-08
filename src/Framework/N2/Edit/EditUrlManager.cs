using System;
using System.Web;
using N2.Configuration;
using N2.Definitions;
using N2.Engine;
using N2.Web;
using N2.Edit.Versioning;

namespace N2.Edit
{
    [Service(typeof(IEditUrlManager))]
    public class EditUrlManager : IEditUrlManager
    {
        private string editTreeUrl;
        private string mediaBrowserUrl;
        private ViewPreference defaultViewPreference;
        private IUrlParser parser;
        private bool ensureLocalhostPreviewUrls;

        public EditUrlManager(IUrlParser parser, EditSection config)
        {
            this.parser = parser;

            ManagementInterfaceUrl = config.Paths.ManagementInterfaceUrl.TrimEnd('/');
            EditTreeUrl = config.Paths.EditTreeUrl;
            EditItemUrl = config.Paths.EditItemUrl;
            EditInterfaceUrl = config.Paths.EditInterfaceUrl;
            if (config.Legacy)
                EditInterfaceUrl += "Content/";
            NewItemUrl = config.Paths.NewItemUrl;
            DeleteItemUrl = config.Paths.DeleteItemUrl;
            defaultViewPreference = config.Versions.DefaultViewMode;
            MediaBrowserUrl = config.Paths.MediaBrowserUrl;
            ensureLocalhostPreviewUrls = config.Paths.EnsureLocalhostPreviewUrls;
        }

        protected virtual string EditInterfaceUrl { get; set; }

        protected virtual string ManagementInterfaceUrl { get; set; }

        protected virtual string DeleteItemUrl { get; set; }

        protected virtual string NewItemUrl { get; set; }

        protected virtual string EditItemUrl { get; set; }

        public virtual string EditTreeUrl
        {
            get { return Url.ResolveTokens(editTreeUrl); }
            set { editTreeUrl = value; }
        }

        public virtual string MediaBrowserUrl
        {
            get { return Url.ResolveTokens(mediaBrowserUrl); }
            set { mediaBrowserUrl = value; }
        }



        /// <summary>Gets the url for the navigation frame.</summary>
        /// <param name="selectedItem">The currently selected item.</param>
        /// <returns>An url.</returns>
        public virtual string GetNavigationUrl(ContentItem selectedItem)
        {
            if (selectedItem == null)
                return null;

            return Url.Parse(EditTreeUrl).AppendQuery(SelectionUtility.SelectedQueryKey, selectedItem.Path);
        }

        /// <summary>Gets the url for the preview frame.</summary>
        /// <param name="selectedItem">The currently selected item.</param>
        /// <returns>An url.</returns>
        public virtual string GetPreviewUrl(ContentItem selectedItem)
        {
            try
            {
                // If hostname == localhost, then don't use custom hostnames in the management navigation tree
                if (ensureLocalhostPreviewUrls && HttpContext.Current != null && HttpContext.Current.Request.Url.Host == "localhost")
                    return selectedItem.FindPath(PathData.DefaultAction).GetRewrittenUrl();

                Url url = ResolveResourceUrl(parser.BuildUrl(selectedItem));
                return url;
            }
            catch (N2Exception)
            {
                return Url.ResolveTokens("{ManagementUrl}/Empty.aspx?n2item=" + selectedItem.ID);
            }
        }

        /// <summary>Gets the url to the edit interface.</summary>
        /// <returns>The url to the edit interface.</returns>
        public virtual string GetEditInterfaceUrl(ViewPreference preference = ViewPreference.Published)
        {
            return Url.ResolveTokens(EditInterfaceUrl).ToUrl().AppendViewPreference(preference, defaultViewPreference);
        }

        /// <summary>Gets the url to the edit interface.</summary>
        /// <returns>The url to the edit interface.</returns>
        public virtual string GetManagementInterfaceUrl()
        {
            return Url.ResolveTokens(ManagementInterfaceUrl + "/");
        }

        /// <summary>Gets the url to the given resource underneath the management interface.</summary>
        /// <returns>The url to the given resource rebased to the management path when not aboslute.</returns>
        public virtual string ResolveResourceUrl(string resourceUrl)
        {
            resourceUrl = resourceUrl ?? String.Empty;
            string finalUrl = resourceUrl;

            finalUrl = Url.ResolveTokens(finalUrl);

            string managementUrl = Url.ResolveTokens(ManagementInterfaceUrl);
            if (finalUrl.StartsWith("~") == false
                && finalUrl.StartsWith("/") == false
                && finalUrl.StartsWith("{") == false
                && finalUrl.StartsWith("javascript:") == false
                && finalUrl.Contains(":") == false
                && finalUrl.StartsWith(managementUrl, StringComparison.InvariantCultureIgnoreCase) == false)
                finalUrl = managementUrl + "/" + resourceUrl.TrimStart('/');

            return Url.ToAbsolute(finalUrl);
        }

        /// <summary>Gets the url to the edit interface.</summary>
        /// <param name="selectedItem">The item to select in edit mode.</param>
        /// <returns>The url to the edit interface.</returns>
        public virtual string GetEditInterfaceUrl(ContentItem selectedItem)
        {
            return FormatSelectedUrl(selectedItem, EditInterfaceUrl);
        }

        /// <summary>Gets the url to the select type of item to create.</summary>
        /// <param name="selectedItem">The currently selected item.</param>
        /// <returns>The url to the select new item to create page.</returns>
        public virtual string GetSelectNewItemUrl(ContentItem selectedItem)
        {
            return FormatSelectedUrl(selectedItem, NewItemUrl);
        }

        /// <summary>Gets the url to the select type of item to create.</summary>
        /// <param name="selectedItem">The currently selected item.</param>
        /// <param name="zoneName">The zone in which to create the item (typically parts)</param>
        /// <returns>The url to the select new item to create page.</returns>
        public string GetSelectNewItemUrl(ContentItem selectedItem, string zoneName)
        {
            return FormatSelectedUrl(selectedItem, NewItemUrl + "?zoneName=" + zoneName);
        }

        /// <summary>Gets the url to the delete item page.</summary>
        /// <param name="selectedItem">The currently selected item.</param>
        /// <returns>The url to the delete page.</returns>
        public virtual string GetDeleteUrl(ContentItem selectedItem)
        {
            return FormatSelectedUrl(selectedItem, DeleteItemUrl);
        }

        /// <summary>Gets the url to edit page creating new items.</summary>
        /// <param name="selected">The selected item.</param>
        /// <param name="definition">The type of item to edit.</param>
        /// <param name="zoneName">The zone to add the item to.</param>
        /// <param name="position">The position relative to the selected item to add the item.</param>
        /// <returns>The url to the edit page.</returns>
        public virtual string GetEditNewPageUrl(ContentItem selected, ItemDefinition definition, string zoneName,
                                                CreationPosition position)
        {
            if (selected == null) throw new ArgumentNullException("selected");
            if (definition == null) throw new ArgumentNullException("definition");

            ContentItem parent = (position != CreationPosition.Below)
                                    ? selected.Parent
                                    : selected;

            if (selected == null)
                throw new N2Exception("Cannot insert item before or after the root page.");

            Url url = Url.ResolveTokens(EditItemUrl);
            url = url.AppendQuery(SelectionUtility.SelectedQueryKey, parent.Path);
            url = url.AppendQuery("discriminator", definition.Discriminator);
            if (!string.IsNullOrEmpty(zoneName))
                url = url.AppendQuery("zoneName", zoneName);
            if (!string.IsNullOrEmpty(definition.TemplateKey))
                url = url.AppendQuery("template", definition.TemplateKey);

            if (position == CreationPosition.Before)
                url = url.AppendQuery("before", selected.Path);
            else if (position == CreationPosition.After)
                url = url.AppendQuery("after", selected.Path);

            if (selected.VersionOf.HasValue)
            {
                url = url.AppendQuery(PathData.VersionIndexQueryKey, selected.VersionIndex)
                    .AppendQuery("VersionKey", selected.GetVersionKey());
            }

            return url;
        }

        /// <summary>Gets the url to the edit page where to edit an existing item.</summary>
        /// <param name="item">The item to edit.</param>
        /// <returns>The url to the edit page</returns>
        public virtual string GetEditExistingItemUrl(ContentItem item, string returnUrl = null)
        {
            if (item == null)
                return null;

            var editUrl = EditItemUrl.ToUrl()
                .ResolveTokens()
                .SetQueryParameter(SelectionUtility.SelectedQueryKey, item.Path);

            if (item.VersionOf.HasValue)
            {
                if (item.IsPage)
                    editUrl = editUrl
                        .SetQueryParameter(PathData.VersionIndexQueryKey, item.VersionIndex)
                        .SetQueryParameter(PathData.VersionKeyQueryKey, item.GetVersionKey());
                else
                    editUrl = editUrl
                        .SetQueryParameter(PathData.VersionIndexQueryKey, Find.ClosestPage(item).VersionIndex)
                        .SetQueryParameter(PathData.VersionKeyQueryKey, item.GetVersionKey());
            }
            else if (item.ID == 0)
            {
                var page = Find.ClosestPage(item);
                if (page != null && page.VersionOf.HasValue)
                    editUrl = editUrl
                        .SetQueryParameter(SelectionUtility.SelectedQueryKey, page.VersionOf.Path)
                        .SetQueryParameter(PathData.VersionIndexQueryKey, page.VersionIndex)
                        .SetQueryParameter(PathData.VersionKeyQueryKey, item.GetVersionKey());
            }

			if (returnUrl != null)
				editUrl = editUrl.SetQueryParameter("returnUrl", returnUrl);

            return editUrl;
        }

        private static string FormatSelectedUrl(ContentItem selectedItem, string path)
        {
            Url url = Url.ResolveTokens(path);
            if (selectedItem != null)
            {
                if (selectedItem.ID == 0 && !selectedItem.VersionOf.HasValue && !selectedItem.IsPage)
                {
                    var page = Find.ClosestPage(selectedItem);
                    return url.AppendQuery(SelectionUtility.SelectedQueryKey + "=" + page.Path)
                        .AppendQuery(PathData.VersionIndexQueryKey + "=" + page.VersionIndex)
                        .AppendQuery(PathData.VersionKeyQueryKey, selectedItem.GetVersionKey());
                }

                url = url.AppendQuery(SelectionUtility.SelectedQueryKey + "=" + selectedItem.Path);
                if (selectedItem.VersionOf.HasValue)
                    url = url.AppendQuery(PathData.VersionIndexQueryKey + "=" + selectedItem.VersionIndex)
                        .AppendQuery(PathData.VersionKeyQueryKey, selectedItem.GetVersionKey());

            }
            return url;
        }
    }
}
