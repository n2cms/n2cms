using System;
using System.Web;
using N2.Configuration;
using N2.Definitions;
using N2.Engine;
using N2.Web;

namespace N2.Edit
{
	[Service(typeof(IEditUrlManager))]
	public class EditUrlManager : IEditUrlManager
	{
		public EditUrlManager(EditSection config)
		{
			EditTreeUrlFormat = "{1}?selected={0}";
			EditPreviewUrlFormat = config.EditPreviewUrlFormat;
			ManagementInterfaceUrl = config.ManagementInterfaceUrl.TrimEnd('/');
			EditTreeUrl = ResolveResourceUrl(config.EditTreeUrl);
			EditItemUrl = ResolveResourceUrl(config.EditItemUrl);
			EditInterfaceUrl = ResolveResourceUrl(config.EditInterfaceUrl);
			NewItemUrl = ResolveResourceUrl(config.NewItemUrl);
			DeleteItemUrl = ResolveResourceUrl(config.DeleteItemUrl);
		}

		protected virtual string EditInterfaceUrl { get; set; }

		protected virtual string ManagementInterfaceUrl { get; set; }

		protected virtual string DeleteItemUrl { get; set; }

		protected virtual string NewItemUrl { get; set; }

		protected virtual string EditItemUrl { get; set; }

		/// <summary>Gets an alternative tree url format when edit mode is displayed.</summary>
		/// <remarks>Accepted format value is {0} for url encoded selected item.</remarks>
		protected virtual string EditTreeUrlFormat { get; set; }

		/// <summary>Gets an alternative preview url format displayed when edit page is loaded.</summary>
		/// <remarks>Accepted format values are {0} for selected page and {1} for url encoded selected item.</remarks>
		protected virtual string EditPreviewUrlFormat { get; set; }

		public virtual string EditTreeUrl { get; set; }

		/// <summary>Gets the url for the navigation frame.</summary>
		/// <param name="selectedItem">The currently selected item.</param>
		/// <returns>An url.</returns>
		public virtual string GetNavigationUrl(INode selectedItem)
		{
			if (selectedItem == null)
				return null;

			return Url.ToAbsolute(string.Format(EditTreeUrlFormat, selectedItem.Path, EditTreeUrl));
		}

		/// <summary>Gets the url for the preview frame.</summary>
		/// <param name="selectedItem">The currently selected item.</param>
		/// <returns>An url.</returns>
		public virtual string GetPreviewUrl(INode selectedItem)
		{
			string url = String.Format(EditPreviewUrlFormat,
			                           selectedItem.PreviewUrl,
			                           HttpUtility.UrlEncode(selectedItem.PreviewUrl)
				);
			return ResolveResourceUrl(url);
		}

		/// <summary>Gets the url to the edit interface.</summary>
		/// <returns>The url to the edit interface.</returns>
		public virtual string GetEditInterfaceUrl()
		{
			return Url.ToAbsolute(EditInterfaceUrl);
		}

		/// <summary>Gets the url to the edit interface.</summary>
		/// <returns>The url to the edit interface.</returns>
		public virtual string GetManagementInterfaceUrl()
		{
			return Url.ToAbsolute(ManagementInterfaceUrl + "/");
		}

		/// <summary>Gets the url to the given resource underneath the management interface.</summary>
		/// <returns>The url to the given resource rebased to the management path when not aboslute.</returns>
		public virtual string ResolveResourceUrl(string resourceUrl)
		{
			resourceUrl = resourceUrl ?? String.Empty;
			string finalUrl = resourceUrl;

			if (resourceUrl.Contains("{ManagementUrl}"))
				finalUrl = resourceUrl.Replace("{ManagementUrl}", ManagementInterfaceUrl.TrimEnd('/'));

			if (finalUrl.StartsWith("~") == false && finalUrl.StartsWith("/") == false
			    && finalUrl.StartsWith(ManagementInterfaceUrl, StringComparison.InvariantCultureIgnoreCase) == false)
				finalUrl = ManagementInterfaceUrl + "/" + resourceUrl.TrimStart('/');

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

			Url url = EditItemUrl;
			url = url.AppendQuery("selected", parent.Path);
			url = url.AppendQuery("discriminator", definition.Discriminator);
			url = url.AppendQuery("zoneName", zoneName);

			if (position == CreationPosition.Before)
				url = url.AppendQuery("before", selected.Path);
			else if (position == CreationPosition.After)
				url = url.AppendQuery("after", selected.Path);
			return url;
		}

		/// <summary>Gets the url to the edit page where to edit an existing item.</summary>
		/// <param name="item">The item to edit.</param>
		/// <returns>The url to the edit page</returns>
		public virtual string GetEditExistingItemUrl(ContentItem item)
		{
			if (item == null)
				return null;

			if (item.VersionOf != null)
				return string.Format("{0}?selectedUrl={1}", EditItemUrl,
				                     HttpUtility.UrlEncode(item.FindPath(PathData.DefaultAction).RewrittenUrl));

			return string.Format("{0}?selected={1}", EditItemUrl, item.Path);
		}

		private static string FormatSelectedUrl(ContentItem selectedItem, string path)
		{
			Url url = Url.ToAbsolute(path);
			if (selectedItem != null)
				url = url.AppendQuery("selected=" + selectedItem.Path);
			return url;
		}
	}
}