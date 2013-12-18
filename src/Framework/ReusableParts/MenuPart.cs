using System.IO;
using System.Web.UI;
using System.Xml.Linq;
using N2.Definitions;
using N2.Details;
using N2.Engine;
using N2.Integrity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web.Mvc;
using N2.Web.UI;
using N2.Web.Parts;

namespace N2.Web
{
	public abstract class MenuPartBase : ContentItem
	{
		public const string CssContainerName = "cssContainer";
		public const string NestingContainerName = "nestingContainer";

		public enum SibilingDisplayOptions
		{
			Always,
			Never,
			OnlyIfItemHasNoChildren
		}

		public enum TitleDisplayOptions
		{
			CustomTitle,
			CurrentPageTitle,
			None
		}

		public abstract TitleDisplayOptions MenuTitleDisplayMode { get; set; }
		public abstract HeadingLevel MenuTitleHeadingLevel { get; set; }
		public abstract int MenuNumChildLevels { get; set; }
		public abstract int MenuStartFromLevel { get; set; }
		public abstract bool MenuShowTreeRoot { get; set; }
		public abstract SibilingDisplayOptions MenuShowSibilings { get; set; }
		public abstract bool MenuFlattenTopLevel { get; set; }
		public abstract bool MenuDontLinkTopLevel { get; set; }
		public abstract bool MenuShowCaretOnItemsWithChildren { get; set; }
		public abstract string MenuOuterUlCssClass { get; set; }
		public abstract string MenuInnerUlCssClass { get; set; }
		public abstract string MenuLiCssClass { get; set; }
		public abstract string MenuSelectedLiCssClass { get; set; }
		public abstract string MenuAncestorLiCssClass { get; set; }
		public abstract bool MenuShowCurrentItemIfHidden { get; set; }
		public abstract bool MenuShowInvisible { get; set; }
		public abstract bool MenuNestChildUls { get; set; }
	}

	/// <summary>
	/// Provides navigation across child and sibiling pages. Ideal for sidebars. 
	/// </summary>
	[PartDefinition(Title = "Menu", IconClass = "n2-icon-list-ul", RequiredPermission = N2.Security.Permission.Administer)]
	[WithEditableTitle]
	[WithEditableName(HelpText = "The name will be used to set the HTML id of the part")]
	[FieldSetContainer(NestingContainerName, "Hierarchy View Settings", 400)]
	[FieldSetContainer(CssContainerName, "Developer: Stylesheets", 500)]
	public class MenuPart : MenuPartBase, IPart
	{
		#region Title Display

		[EditableEnum("Title Display Mode", 100, typeof(TitleDisplayOptions))]
		public override TitleDisplayOptions MenuTitleDisplayMode
		{
			get { return GetDetail("TitleDisplay", TitleDisplayOptions.None); }
			set { SetDetail("TitleDisplay", value); }
		}

		[EditableEnum("Title Heading Format", 150, typeof(HeadingLevel))]
		public override HeadingLevel MenuTitleHeadingLevel
		{
			get { return GetDetail("TitleMode", HeadingLevel.H1); }
			set { SetDetail("TitleMode", value); }
		}

		#endregion


		#region Hierarchy Options

		[EditableNumber("Num Child Levels", 400, MinimumValue = "0", Required = true, ContainerName = NestingContainerName)]
		public override int MenuNumChildLevels
		{
			get { return GetDetail("Levels", 1); }
			set { SetDetail("Levels", value); }
		}

		[EditableNumber("Start from Level", 420,
			Required = true,
			HelpText = "Positive values are absolute from root; negative values are relative-up from current item.",
			ContainerName = NestingContainerName)]
		public override int MenuStartFromLevel
		{
			get { return GetDetail("StartLevel", 2); }
			set { SetDetail("StartLevel", value); }
		}

		[EditableCheckBox("Show the root node of the tree.", 460, ContainerName = NestingContainerName,
			HelpText = "Disabling the root node of a tree can be usefull when configuring the menu from a positive start level and only wanting to see its children on the root page of the menu.")]
		public override bool MenuShowTreeRoot
		{
			get { return GetDetail("ShowTreeRoot", true); }
			set { SetDetail("ShowTreeRoot", value); }
		}

		[EditableEnum(typeof(SibilingDisplayOptions), Title = "Show Sibilings", ContainerName = NestingContainerName)]
		public override SibilingDisplayOptions MenuShowSibilings
		{
			get { return GetDetail("ShowSibilings", SibilingDisplayOptions.OnlyIfItemHasNoChildren); }
			set { SetDetail("ShowSibilings", value); }
		}

		[EditableCheckBox("Flatten top-level items to list-headers", 460, ContainerName = NestingContainerName)]
		public override bool MenuFlattenTopLevel
		{
			get { return GetDetail("ftl", false); }
			set { SetDetail("ftl", value); }
		}

		[EditableCheckBox("Do not link top-level items", 470, ContainerName = NestingContainerName)]
		public override bool MenuDontLinkTopLevel
		{
			get { return GetDetail("nll", false); }
			set { SetDetail("nll", value); }
		}

		[EditableCheckBox("Show caret on items with children", 480, ContainerName = NestingContainerName)]
		public override bool MenuShowCaretOnItemsWithChildren
		{
			get { return GetDetail("sc", false); }
			set { SetDetail("sc", value); }
		}

		[EditableCheckBox("Show current item even if item is hidden", 490, ContainerName = NestingContainerName)]
		public override bool MenuShowCurrentItemIfHidden
		{
			get { return GetDetail("MenuShowCurrentItemIfHidden", false); }
			set { SetDetail("MenuShowCurrentItemIfHidden", value); }
		}

		[EditableCheckBox("Show items that aren't normally visible in navigation", 491, ContainerName = NestingContainerName)]
		public override bool MenuShowInvisible
		{
			get { return GetDetail("ShowInvisible", false); }
			set { SetDetail("ShowInvisible", value); }
		}

		[EditableCheckBox("Nest child UL elements inside the parent LI element", 495, ContainerName = NestingContainerName)]
		public override bool MenuNestChildUls
		{
			get { return GetDetail("NestChildUls", false); }
			set { SetDetail("NestChildUls", value); }
		}

		#endregion


		#region CSS Overrides

		[EditableText("Outer UL CssClass", 500, ContainerName = CssContainerName)]
		public override string MenuOuterUlCssClass
		{
			get { return GetDetail("OuterUlClass", "nav nav-stacked"); }
			set { SetDetail("OuterUlClass", value); }
		}

		[EditableText("Inner UL CssClass", 510, ContainerName = CssContainerName)]
		public override string MenuInnerUlCssClass
		{
			get { return GetDetail("InnerUlClass", "nav nav-stacked nav-inner"); }
			set { SetDetail("InnerUlClass", value); }
		}

		[EditableText("Item CssClass", 520, ContainerName = CssContainerName)]
		public override string MenuLiCssClass
		{
			get { return GetDetail("LiCssClass", "nav-item"); }
			set { SetDetail("LiCssClass", value); }
		}

		[EditableText("Selected Item CssClass", 530, ContainerName = CssContainerName)]
		public override string MenuSelectedLiCssClass
		{
			get { return GetDetail("SelectedLiCssClass", "nav-item active"); }
			set { SetDetail("SelectedLiCssClass", value); }
		}

		[EditableText("Acestor Item CssClass", 540, ContainerName = CssContainerName)]
		public override string MenuAncestorLiCssClass
		{
			get { return GetDetail("AncestorLiCssClass", "ancestor"); }
			set { SetDetail("AncestorLiCssClass", value); }
		}

		#endregion

	}

	/// <summary>
	/// Renders the SubNavigation part using the ASP.NET MVC framework.
	/// </summary>
	[Adapts(typeof(MenuPart))]
	public class MenuPartMvcAdapter : PartsAdapter
	{
		public override void RenderPart(System.Web.Mvc.HtmlHelper html, ContentItem part, TextWriter writer = null)
		{
			var menuPart = part as MenuPart;
			if (part == null)
				throw new ArgumentException("part");

			if (html.ViewContext.Writer is HtmlTextWriter)
				(new MenuPartRenderer(menuPart)).WriteHtml(html.ViewContext.Writer as HtmlTextWriter);
			else
				html.ViewContext.Writer.Write(new MenuPartRenderer(menuPart).GetHtml());
		}
	}



	//TODO: Write a WebForms compatible adapter for MenuPart.

	/// <summary>Handles rendering of MenuParts for both MVC and Webforms.</summary>
	public sealed class MenuPartRenderer
	{

		private class ContentTreeNode
		{
			public ContentTreeNode(ContentItem item, ContentTreeNode parent)
			{
				Item = item;
				Parent = parent;
				SortOrder = item.SortOrder;
				ItemId = item.ID;
			}

			public int ItemId { get; protected set; }
			public int SortOrder { get; protected set; }
			public ContentItem Item { get; private set; }
			public ContentTreeNode Parent { get; private set; }
			public bool IsAncestor { get; set; }
		}

		private readonly List<ContentTreeNode> database;
		private readonly MenuPart menuPart;
		private int cId;

		public MenuPartRenderer(MenuPart menuPart)
		{
			this.menuPart = menuPart;
			var cacheKey = String.Concat(N2.Context.CurrentPage.ID.ToString(), "+", menuPart.AncestralTrail);
			var cacheData = System.Web.Hosting.HostingEnvironment.Cache.Get(cacheKey);
			if (cacheData == null)
			{
				cacheData = BuildNavTree();
				System.Web.Hosting.HostingEnvironment.Cache.Add(cacheKey, cacheData, null, DateTime.Now.AddSeconds(15),
					System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
			}
			database = (List<ContentTreeNode>)cacheData;
		}

		public string GetHtml()
		{
			var sb = new StringBuilder();
			using (var sw = new StringWriter(sb))
			using (var xml = new XhtmlTextWriter(sw))
				WriteHtml(xml);
			return sb.ToString();
		}

		public void WriteHtml(HtmlTextWriter xml)
		{
			xml.AddAttribute("id", String.IsNullOrEmpty(menuPart.Name) ? "menu" + menuPart.ID : menuPart.Name);
			xml.AddAttribute("class", "menuNavPart");
			xml.RenderBeginTag("div");

			switch (menuPart.MenuTitleDisplayMode)
			{
				case MenuPartBase.TitleDisplayOptions.CustomTitle:
					xml.WriteLine(HeadingLevelUtility.DoTitle(menuPart.MenuTitleHeadingLevel, menuPart.Title));
					break;
				case MenuPartBase.TitleDisplayOptions.CurrentPageTitle:
					xml.WriteLine(HeadingLevelUtility.DoTitle(menuPart.MenuTitleHeadingLevel, Content.Current.Page.Title));
					break;
				case MenuPartBase.TitleDisplayOptions.None:
					break;
			}

			if (menuPart.MenuFlattenTopLevel)
			{
				WriteListWithHeaders(null, xml);
			}
			else
			{
				WriteChildList(null, xml, 0);
			}

			xml.RenderEndTag(); // </div>
		}

		private IEnumerable<ContentItem> GetChildren(ContentItem ancestorItem)
		{
			return (from x in ancestorItem.GetChildren()
					where x.IsPage && (menuPart.MenuShowInvisible || x.Visible) && x.IsPublished() && x.ID != Content.Current.Page.ID
					select x).ToArray();
		}

		/// <summary>Determines if the given ContentItem has children that are visible to this MenuPart.</summary>
		/// <param name="ancestorItem">The (potentially) parent item to check for children</param>
		/// <returns>Returns True if the given ContentItem has children that are visible to this MenuPart.</returns>
		private bool HasVisibleChildren(ContentItem ancestorItem)
		{
			return menuPart.MenuShowInvisible
				? Content.Search.PublishedPages.Any(item => item.Parent == ancestorItem)
				: Content.Search.PublishedPages.Any(item => item.Parent == ancestorItem && item.Visible);
		}

		private List<ContentTreeNode> BuildNavTree()
		{
			List<ContentTreeNode> navTree = new List<ContentTreeNode>();
			var ci = Content.Current.Page;
			if (ci == null)
				return navTree;
			cId = ci.ID; // need to cache this due to the following if clause:
			if (ci.VersionOf != null && ci.VersionOf.Value != null)
				ci = ci.VersionOf.Value; // get the published version

			// follow the ancestral trail up to the desired "start from level"
			var convertedAncestralTrail = Array.ConvertAll(ci.AncestralTrail.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries), int.Parse);

			var xn = menuPart.MenuStartFromLevel;
			if (xn < 0)
				xn += convertedAncestralTrail.Length; // handle "zero" case

			var expandedParents = new List<ContentTreeNode>();
			for (var i = Math.Max(xn, 0); i < convertedAncestralTrail.Length; ++i)
			{
				var ancestorItem = Context.Current.Persister.Get(Convert.ToInt32(convertedAncestralTrail[i]));
				var ancestorNode = new ContentTreeNode(ancestorItem, navTree.LastOrDefault()) { IsAncestor = true };
				navTree.Add(ancestorNode);

				// expand the ancestor
				// ReSharper disable LoopCanBeConvertedToQuery
				foreach (var item in GetChildren(ancestorItem))
					expandedParents.Add(new ContentTreeNode(item, ancestorNode));
				// ReSharper restore LoopCanBeConvertedToQuery
			}


			// Add current item ================================================================

			if (ci.Visible || menuPart.MenuShowCurrentItemIfHidden)
			{
				// -- add a node for the current page --
				var navItemCurrent = new ContentTreeNode(ci, navTree.LastOrDefault());
				var navItemCParent = navTree.LastOrDefault();
				navTree.Add(navItemCurrent);

				// -- get children and sibilings --
				// ReSharper disable LoopCanBeConvertedToQuery
				foreach (var child in GetChildren(ci).OrderBy(f => f.SortOrder))
					navTree.Add(new ContentTreeNode(child, navItemCurrent));
				// ReSharper restore LoopCanBeConvertedToQuery
			}



			// add the ancestors we just expanded 
			// ReSharper disable LoopCanBeConvertedToQuery
			foreach (var item in expandedParents)
				if (navTree.All(f => f.ItemId != item.ItemId))
					navTree.Add(item);
			// ReSharper restore LoopCanBeConvertedToQuery



			// show sibilings of the current item (put under navItemCParent)
			//if (menuPart.ShowSibilings != MenuPartBase.SibilingDisplayOptions.Never)
			//{
			//	if (menuPart.ShowSibilings == MenuPartBase.SibilingDisplayOptions.Always
			//		|| (menuPart.ShowSibilings == MenuPartBase.SibilingDisplayOptions.OnlyIfItemHasNoChildren && chil.Length > 0))
			//	{
			//		// ok...
			//		// ReSharper disable LoopCanBeConvertedToQuery
			//		foreach (var sibiling in sibs.OrderBy(f => f.SortOrder))
			//			navTree.Add(new ContentTreeNode(sibiling, navItemCParent));
			//		// ReSharper restore LoopCanBeConvertedToQuery
			//	}
			//}

			return navTree;
		}


		// ReSharper disable ParameterTypeCanBeEnumerable.Local
		private void WriteChildList(ContentTreeNode currentNode, HtmlTextWriter xml, int level)
		{
			var childNodes = database.Where(f => f.Parent == currentNode).ToList();
			if (childNodes.Count <= 0) return;

			xml.AddAttribute("class", currentNode == null ? menuPart.MenuOuterUlCssClass : menuPart.MenuInnerUlCssClass);
			xml.RenderBeginTag(HtmlTextWriterTag.Ul);

			if (currentNode == null && (!menuPart.MenuShowTreeRoot && childNodes.Count > 0)) // indicates that we are starting the menu, 
			{
				// Skip directly to the childre of the root node.
				childNodes = database.Where(f => f.Parent == childNodes.First()).ToList();
			}

			foreach (var childNode in childNodes.OrderBy(n => n.SortOrder).ThenBy(n => n.Item.ID))
			{
				WriteListItem(childNode, xml, level, null);

				if (!menuPart.MenuNestChildUls)
				{
					WriteChildList(childNode, xml, level + 1);
				}
			}
			xml.RenderEndTag();
		}
		// ReSharper restore ParameterTypeCanBeEnumerable.Local

		/// <summary>
		/// Writes the top two levels of a tree flattened into a single list, with header styles on the level-1 list
		/// items, and with sub-items in their respective hierarchies. 
		/// </summary>
		/// <param name="menuPart"></param>
		/// <param name="database"></param>
		/// <param name="currentNode"></param>
		/// <param name="xml"></param>
		private void WriteListWithHeaders(ContentTreeNode currentNode, HtmlTextWriter xml)
		{
			var childNodes = database.Where(f => f.Parent == currentNode).ToList();
			if (childNodes.Count <= 0) return;

			xml.AddAttribute("class", currentNode == null ? menuPart.MenuOuterUlCssClass : menuPart.MenuInnerUlCssClass);
			xml.RenderBeginTag(HtmlTextWriterTag.Ul);
			foreach (var childNode in childNodes.OrderBy(n => n.SortOrder).ThenBy(n => n.Item.ID))
			{
				WriteListItem(childNode, xml, 0, "nav-header disabled"); // header item

				var childNodes2 = database.Where(f => f.Parent == childNode).OrderBy(n => n.SortOrder).ToList();
				foreach (var childnode2 in childNodes2)
				{
					WriteListItem(childnode2, xml, 1, null);

					if (!menuPart.MenuNestChildUls)
					{
						WriteChildList(childNode, xml, 2);
					}
				}
			}
			xml.RenderEndTag();
		}

		private void WriteListItem(ContentTreeNode childNode, HtmlTextWriter xml, int level, string cssClass)
		{
			// write LI...
			var sn = menuPart;
			var childItem = childNode.Item;
			var isSelected = childItem.ID == cId;

			if (cssClass == null)
				cssClass = childNode.IsAncestor
					? sn.MenuAncestorLiCssClass
					: isSelected ? sn.MenuSelectedLiCssClass : sn.MenuLiCssClass;

			xml.AddAttribute("class", cssClass);
			xml.RenderBeginTag(HtmlTextWriterTag.Li);

			if (isSelected || (level == 0 && sn.MenuDontLinkTopLevel))
			{
				xml.RenderBeginTag(HtmlTextWriterTag.A); // wrap in a dummy <a> for Bootstrap 3, http://stackoverflow.com/questions/18329010/what-happened-to-nav-header-class
				xml.Write(childItem.Title);
				xml.RenderEndTag();
			}
			else
			{
				xml.AddAttribute("href", childItem.Url);
				xml.RenderBeginTag(HtmlTextWriterTag.A);
				xml.Write(childItem.Title);

				// render caret if subitems exist
				if (sn.MenuShowCaretOnItemsWithChildren
					&& HasVisibleChildren(childItem))
				{
					// <b class="caret"></b> 
					xml.Write(' ');
					xml.AddAttribute("class", "caret");
					xml.RenderBeginTag(HtmlTextWriterTag.B);
					xml.RenderEndTag();
				}
				xml.RenderEndTag();
			}

			if (menuPart.MenuNestChildUls)
			{
				WriteChildList(childNode, xml, level + 1);
			}

			xml.RenderEndTag(); // </li>
			xml.WriteLine();
		}

	}

}
