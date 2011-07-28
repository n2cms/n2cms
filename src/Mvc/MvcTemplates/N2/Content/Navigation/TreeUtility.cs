using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using N2.Collections;
using N2.Engine;
using N2.Web;
using N2.Edit;
using System.Web;
using System.Web.Mvc;

namespace N2.Management.Content.Navigation
{
	class TreeUtility
	{
		internal static void Write(HierarchyNode<ContentItem> hierarchy, ContentItem selectedItem, IContentAdapterProvider adapters, ItemFilter filter, string selectableTypes, string selectableExtensions, bool excludeRoot, string target, TextWriter writer)
		{
			N2.Web.Tree.Using(hierarchy)
				.OpenTo(selectedItem)
				.Filters(filter)
				.IdProvider(n => "u" + n.Current.ID, n => "l" + n.Current.ID)
				.LinkWriter((n, w) =>
				{
					BuildLink(adapters.ResolveAdapter<NodeAdapter>(n.Current), n.Current, n.Current.Path == selectedItem.Path, target, IsSelectable(n.Current, selectableTypes, selectableExtensions)).WriteTo(w);
					if (n.Children.Count == 0 && adapters.ResolveAdapter<NodeAdapter>(n.Current).HasChildren(n.Current, filter))
					{
						var ul = new TagBuilder("ul");
						ul.AddCssClass("ajax");
						w.Write(ul.ToString(TagRenderMode.StartTag));

						var li = new TagBuilder("li");
						li.InnerHtml = "{url:" + Url.ParseTokenized("{ManagementUrl}/Content/Navigation/LoadTree.ashx")
							.AppendQuery("target", target)
							.AppendQuery("selected", HttpUtility.UrlEncode(n.Current.Path))
							.AppendQuery("selectableTypes", selectableTypes)
							.AppendQuery("selectableExtensions", selectableExtensions)
							+ "}";
						w.Write(li.ToString());

						w.Write(ul.ToString(TagRenderMode.EndTag));
					}
				})
				.ExcludeRoot(excludeRoot)
				.WriteTo(writer);
		}

		internal static ILinkBuilder BuildLink(NodeAdapter adapter, ContentItem item, bool isSelected, string target, bool isSelectable)
		{
			INode node = item;
			string className = node.ClassNames;
			if (isSelected)
				className += "selected ";
			if (isSelectable)
				className += "selectable ";
			else
				className += "unselectable ";

			ILinkBuilder builder = Link.To(node)
				.Target(target)
				.Class(className)
				.Href(adapter.GetPreviewUrl(item))
				.Text("<img src='" + adapter.GetIconUrl(item) + "'/>" + node.Contents)
				.Attribute("id", item.Path.Replace('/', '_'))
				.Attribute("title", "#" + item.ID + ": " + N2.Context.Current.Definitions.GetDefinition(item).Title)
				.Attribute("data-id", item.ID.ToString())
				.Attribute("data-type", item.GetContentType().Name)
				.Attribute("data-path", item.Path)
				.Attribute("data-url", item.Url)
				.Attribute("data-page", item.IsPage.ToString().ToLower())
				.Attribute("data-zone", item.ZoneName)
				.Attribute("data-permission", adapter.GetMaximumPermission(item).ToString());

			if (isSelected)
				builder.Attribute("data-selected", "true");
			if (isSelectable)
				builder.Attribute("data-selectable", "true");

			builder.Href(adapter.GetPreviewUrl(item));

			return builder;
		}

		internal static bool IsSelectable(ContentItem item, string selectableTypes, string selectableExtensions)
		{
			var baseTypesAndInterfaceNames = item.GetContentType().GetInterfaces().Select(i => i.Name)
				.Union(Utility.GetBaseTypesAndSelf(item.GetContentType()).Select(t => t.Name));

			bool isSelectableType = string.IsNullOrEmpty(selectableTypes)
				|| selectableTypes.Split(',').Intersect(baseTypesAndInterfaceNames).Any();

			bool isSelectableExtension = string.IsNullOrEmpty(selectableExtensions)
				|| selectableExtensions.Split(',').Contains(VirtualPathUtility.GetExtension(item.Url), StringComparer.InvariantCultureIgnoreCase);

			return isSelectableType && isSelectableExtension;
		}

	}
}
