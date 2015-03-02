//#define NO_MENUPART_CACHE
//#define ENABLE_DEBUG_COMMENTS

using System.Globalization;
using System.IO;
using System.Web.UI;
using N2.Definitions;
using N2.Details;
using N2.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web.UI;
using N2.Web.Parts;


namespace N2.Web
{
	/// <summary>
	/// Base class for a MenuPart.
	/// </summary>
	public abstract class MenuPartBase : ContentItem
	{
		public const string CssContainerName = "cssContainer";
		public const string NestingContainerName = "nestingContainer";

		public enum SiblingDisplayOptions
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

		/// <summary>
		/// Gets or sets the menu title display mode.
		/// </summary>
		/// <value>
		/// The menu title display mode.
		/// </value>
		public abstract TitleDisplayOptions MenuTitleDisplayMode { get; set; }

		/// <summary>
		/// Gets or sets the menu title heading level.
		/// </summary>
		/// <value>
		/// The menu title heading level.
		/// </value>
		public abstract HeadingLevel MenuTitleHeadingLevel { get; set; }

		/// <summary>
		/// Gets or sets the number of child levels.
		/// </summary>
		/// <value>
		/// The number of child levels.
		/// </value>
		public abstract int MenuNumChildLevels { get; set; }

		/// <summary>
		/// Gets or sets the menu starting level.
		/// </summary>
		/// <value>
		/// The menu starting level.
		/// </value>
		public abstract int MenuStartFromLevel { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the root node of the tree should be shown (rendered).
		/// </summary>
		/// <value>
		///   <c>true</c> if the root node of the tree should be shown (rendered); otherwise, <c>false</c>.
		/// </value>
		public abstract bool MenuShowTreeRoot { get; set; }

		/// <summary>
		/// Expands the navigation tree so that sibling pages of the current page are always shown.
		/// </summary>
		public abstract SiblingDisplayOptions MenuShowSiblings { get; set; }

		/// <summary>
		/// If this property is set to True, second-level items in the navigation tree are
		/// inserted as siblings of the top-level item(s).
		/// </summary>
		public abstract bool MenuFlattenTopLevel { get; set; }

		/// <summary>
		/// Prevents the top-level item from ever being hyperlinked. This is useful if you want
		/// the top-level item to be a static heading.
		/// </summary>
		public abstract bool MenuDontLinkTopLevel { get; set; }

		/// <summary>
		/// Defines whether a 'caret' suffix is appended to the text of items with children. 
		/// The HTML code for the caret can be overridden by setting the CaretHtml property.
		/// </summary>
		public abstract bool MenuShowCaretOnItemsWithChildren { get; set; }

		/// <summary>
		/// Defines the CSS class that is applied to the top level UL element.
		/// </summary>
		public abstract string MenuOuterUlCssClass { get; set; }

		/// <summary>
		/// Defines the CSS class that is applied to all UL elements below the top level UL.
		/// </summary>
		public abstract string MenuInnerUlCssClass { get; set; }

		/// <summary>
		/// Determines the base CSS class that is applied to all menu items.
		/// </summary>
		public abstract string MenuLiCssClass { get; set; }

		/// <summary>
		/// Determines the CSS class of the menu item for the currently selected page.
		/// </summary>
		public abstract string MenuSelectedLiCssClass { get; set; }

		/// <summary>
		/// Defines the CSS class of menu items for ancestor elements of the current page. 
		/// </summary>
		public abstract string MenuAncestorLiCssClass { get; set; }

		/// <summary>
		/// Determines if the menu will show the current item even if it wouldn't normally be shown.
		/// </summary>
		public abstract bool MenuShowCurrentItemIfHidden { get; set; }

		/// <summary>Determines if items with "Visible in Navigation" (visible property) set to False will be shown in the menu anyways.</summary>
		public abstract bool MenuShowInvisible { get; set; }

		/// <summary>
		/// Determines if child ULs are nested.
		/// </summary>
		public abstract bool MenuNestChildUls { get; set; }

		/// <summary>
		/// Defines the HTML code that is written out for items with children. Set this to NULL and the MenuShowCaretOnItemsWithChildren
		/// property to True to render the default Twitter Bootstrap-compatible caret code.
		/// </summary>
		public abstract string MenuCaretCustomHtml { get; set; }
	}

	/// <summary>
	/// Provides navigation across child and sibling pages. Ideal for sidebars.
	/// </summary>
	[PartDefinition(Title = "Menu", IconClass = "fa fa-list-ul", RequiredPermission = Security.Permission.Administer)]
	[WithEditableTitle]
	[WithEditableName(HelpText = "The name will be used to set the HTML id of the part")]
	[FieldSetContainer(NestingContainerName, "Hierarchy View Settings", 400)]
	[FieldSetContainer(CssContainerName, "Developer: Stylesheets", 500)]
	public class MenuPart : MenuPartBase, IPart
	{
		#region Title Display

		/// <summary>
		/// Gets or sets the menu title display mode.
		/// </summary>
		/// <value>
		/// The menu title display mode.
		/// </value>
		[EditableEnum("Title Display Mode", 100, typeof(TitleDisplayOptions))]
		public override TitleDisplayOptions MenuTitleDisplayMode
		{
			get { return GetDetail("TitleDisplay", TitleDisplayOptions.None); }
			set { SetDetail("TitleDisplay", value); }
		}

		/// <summary>
		/// Gets or sets the menu title heading level.
		/// </summary>
		/// <value>
		/// The menu title heading level.
		/// </value>
		[EditableEnum("Title Heading Format", 150, typeof(HeadingLevel))]
		public override HeadingLevel MenuTitleHeadingLevel
		{
			get { return GetDetail("TitleMode", HeadingLevel.H1); }
			set { SetDetail("TitleMode", value); }
		}

		#endregion


		#region Hierarchy Options

		/// <summary>
		/// Gets or sets the number of child levels the menu renders.
		/// </summary>
		/// <value>
		/// The number of child levels (recursion depth).
		/// </value>
		[EditableNumber("Num Child Levels", 400, MinimumValue = "0", Required = true, ContainerName = NestingContainerName)]
		public override int MenuNumChildLevels
		{
			get { return GetDetail("Levels", 1); }
			set { SetDetail("Levels", value); }
		}

		/// <summary>
		/// Gets or sets the menu start from level.
		/// </summary>
		/// <value>
		/// The menu start from level.
		/// </value>
		[EditableNumber("Start from Level", 420,
			Required = true,
			HelpText = "Positive values are absolute from root; negative values are relative-up from current item.",
			ContainerName = NestingContainerName)]
		public override int MenuStartFromLevel
		{
			get { return GetDetail("StartLevel", 2); }
			set { SetDetail("StartLevel", value); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the root node of the tree should be shown (rendered).
		/// </summary>
		/// <value>
		///   <c>true</c> if the root node of the tree should be shown (rendered); otherwise, <c>false</c>.
		/// </value>
		[EditableCheckBox("Show the root node of the tree.", 460, ContainerName = NestingContainerName,
			HelpText = "Disabling the root node of a tree can be useful when configuring the menu from a positive start level and only wanting to see its children on the root page of the menu.")]
		public override bool MenuShowTreeRoot
		{
			get { return GetDetail("ShowTreeRoot", true); }
			set { SetDetail("ShowTreeRoot", value); }
		}

		/// <summary>
		/// Expands the navigation tree so that sibling pages of the current page are always shown.
		/// </summary>
		[EditableEnum(typeof(SiblingDisplayOptions), Title = "Show Siblings", ContainerName = NestingContainerName)]
		public override SiblingDisplayOptions MenuShowSiblings
		{
			get { return GetDetail("ShowSiblings", SiblingDisplayOptions.OnlyIfItemHasNoChildren); }
			set { SetDetail("ShowSiblings", value); }
		}

		/// <summary>
		/// If this property is set to True, second-level items in the navigation tree are
		/// inserted as siblings of the top-level item(s).
		/// </summary>
		[EditableCheckBox("Flatten top-level items to list-headers", 460, ContainerName = NestingContainerName)]
		public override bool MenuFlattenTopLevel
		{
			get { return GetDetail("ftl", false); }
			set { SetDetail("ftl", value, false); }
		}

		/// <summary>
		/// Prevents the top-level item from ever being hyperlinked. This is useful if you want
		/// the top-level item to be a static heading.
		/// </summary>
		[EditableCheckBox("Do not link top-level items", 470, ContainerName = NestingContainerName)]
		public override bool MenuDontLinkTopLevel
		{
			get { return GetDetail("nll", false); }
			set { SetDetail("nll", value); }
		}

		/// <summary>
		/// Defines whether a 'caret' suffix is appended to the text of items with children.
		/// The HTML code for the caret can be overridden by setting the CaretHtml property.
		/// </summary>
		[EditableCheckBox("Show caret on items with children", 480, ContainerName = NestingContainerName)]
		public override bool MenuShowCaretOnItemsWithChildren
		{
			get { return GetDetail("sc", false); }
			set { SetDetail("sc", value, false); }
		}

		/// <summary>
		/// Determines if the menu will show the current item even if it wouldn't normally be shown.
		/// </summary>
		[EditableCheckBox("Show current item even if item is hidden", 490, ContainerName = NestingContainerName)]
		public override bool MenuShowCurrentItemIfHidden
		{
			get { return GetDetail("MenuShowCurrentItemIfHidden", false); }
			set { SetDetail("MenuShowCurrentItemIfHidden", value, false); }
		}

		/// <summary>
		/// Determines if items with "Visible in Navigation" (visible property) set to False will be shown in the menu anyways.
		/// </summary>
		[EditableCheckBox("Show items that aren't normally visible in navigation", 491, ContainerName = NestingContainerName)]
		public override bool MenuShowInvisible
		{
			get { return GetDetail("ShowInvisible", false); }
			set { SetDetail("ShowInvisible", value, false); }
		}

		/// <summary>
		/// Determines if child ULs are nested.
		/// </summary>
		[EditableCheckBox("Nest child UL elements inside the parent LI element", 495, ContainerName = NestingContainerName)]
		public override bool MenuNestChildUls
		{
			get { return GetDetail("NestChildUls", false); }
			set { SetDetail("NestChildUls", value, false); }
		}

		/// <summary>
		/// Defines the HTML code that is written out for items with children. Set this to NULL and the MenuShowCaretOnItemsWithChildren
		/// property to True to render the default Twitter Bootstrap-compatible caret code.
		/// </summary>
		[EditableText("Caret HTML", 500, HelpText = "Overrides the HTML code for the caret; by default, renders a Twitter Bootstrap caret.", Columns = 20, Rows = 4, TextMode = System.Web.UI.WebControls.TextBoxMode.MultiLine, Required = false)]
		public override string MenuCaretCustomHtml
		{
			get { return GetDetail("MenuCaretCustomHtml", string.Empty); }
			set { SetDetail("MenuCaretCustomHtml", value, string.Empty); }
		}

		#endregion


		#region CSS Overrides

		/// <summary>
		/// Defines the CSS class that is applied to the top level UL element.
		/// </summary>
		[EditableText("Outer UL CssClass", 500, ContainerName = CssContainerName)]
		public override string MenuOuterUlCssClass
		{
			get { return GetDetail("OuterUlClass", "nav nav-stacked"); }
			set { SetDetail("OuterUlClass", value); }
		}

		/// <summary>
		/// Defines the CSS class that is applied to all UL elements below the top level UL.
		/// </summary>
		[EditableText("Inner UL CssClass", 510, ContainerName = CssContainerName)]
		public override string MenuInnerUlCssClass
		{
			get { return GetDetail("InnerUlClass", "nav nav-stacked nav-inner"); }
			set { SetDetail("InnerUlClass", value); }
		}

		/// <summary>
		/// Determines the base CSS class that is applied to all menu items.
		/// </summary>
		[EditableText("Item CssClass", 520, ContainerName = CssContainerName)]
		public override string MenuLiCssClass
		{
			get { return GetDetail("LiCssClass", "nav-item"); }
			set { SetDetail("LiCssClass", value); }
		}

		/// <summary>
		/// Determines the CSS class of the menu item for the currently selected page.
		/// </summary>
		[EditableText("Selected Item CssClass", 530, ContainerName = CssContainerName)]
		public override string MenuSelectedLiCssClass
		{
			get { return GetDetail("SelectedLiCssClass", "nav-item active"); }
			set { SetDetail("SelectedLiCssClass", value); }
		}

		/// <summary>
		/// Defines the CSS class of menu items for ancestor elements of the current page.
		/// </summary>
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

			public int ItemId { get; private set; }
			public int SortOrder { get; private set; }
			public ContentItem Item { get; private set; }
			public ContentTreeNode Parent { get; private set; }
			public bool IsAncestor { get; set; }
		}

		private readonly List<ContentTreeNode> database;
		private readonly MenuPart menuPart;
		private readonly int currentPageId;

		/// <summary>
		/// Initializes a new instance of the <see cref="MenuPartRenderer"/> class.
		/// </summary>
		/// <param name="menuPart">The menu part to be rendered.</param>
		public MenuPartRenderer(MenuPart menuPart)
		{
			this.menuPart = menuPart;
			var page = Context.CurrentPage;

			// Need the current page ID regardless of cached database or not to render selected page correctly.
			currentPageId = page == null ? 0 : page.ID;

#if NO_MENUPART_CACHE
			database = BuildNavigationTree(page);
#else
		    if (page != null)
		    {
		        var cacheKey = String.Concat(page.ID.ToString(CultureInfo.InvariantCulture), "+", menuPart.AncestralTrail);
		        var cacheData = System.Web.Hosting.HostingEnvironment.Cache.Get(cacheKey);
		        if (cacheData == null)
		        {
		            cacheData = BuildNavigationTree(page);
		            System.Web.Hosting.HostingEnvironment.Cache.Add(cacheKey, cacheData, null, DateTime.Now.AddSeconds(15),
		                System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
		        }
		        database = (List<ContentTreeNode>) cacheData;
		    }
		    else
		    {
		        database = new List<ContentTreeNode>();
		    }
#endif
		}

		/// <summary>
		/// Generates HTML code for the given <c>MenuPart</c>.
		/// </summary>
		/// <returns>HTML code as a string</returns>
		public string GetHtml()
		{
			var sb = new StringBuilder();
			using (var sw = new StringWriter(sb))
			using (var xml = new XhtmlTextWriter(sw))
				WriteHtml(xml);
			return sb.ToString();
		}

		/// <summary>
		/// Writes the HTML for the given <c>MenuPart</c> to the specified <c>HtmlTextWriter</c>.
		/// </summary>
		/// <param name="xml">The <c>HtmlTextWriter</c> to write the output.</param>
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

		private IQueryable<ContentItem> QueryApplicableChildren(ContentItem ancestorItem)
		{
			return menuPart.MenuShowInvisible
				? Content.Search.PublishedPages.Where(item => item.ID != Content.Current.Page.ID && item.Parent == ancestorItem)
				: Content.Search.PublishedPages.Where(item => item.ID != Content.Current.Page.ID && item.Parent == ancestorItem && item.Visible);
		}

		/// <summary>
		/// Builds the internal navigation tree structure.
		/// </summary>
		/// <param name="currentPage">The current page.</param>
		/// <returns></returns>
		private List<ContentTreeNode> BuildNavigationTree(ContentItem currentPage)
		{
			var navTree = new List<ContentTreeNode>();
			if (currentPage == null)
				return navTree;

			if (currentPage.VersionOf != null && currentPage.VersionOf.Value != null)
				currentPage = currentPage.VersionOf.Value; // get the published version

			// follow the ancestral trail up to the desired "start from level"
			var convertedAncestralTrail = Array.ConvertAll(currentPage.AncestralTrail.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries), int.Parse);

			var xn = menuPart.MenuStartFromLevel;
			if (xn < 0)
				xn += convertedAncestralTrail.Length; // handle "zero" case

			var expandedParents = new List<ContentTreeNode>();
			for (var i = Math.Max(xn, 0); i < convertedAncestralTrail.Length; ++i)
			{
				var ancestorItem = Context.Current.Persister.Get(convertedAncestralTrail[i]);
				var ancestorNode = new ContentTreeNode(ancestorItem, navTree.LastOrDefault()) { IsAncestor = true };
				navTree.Add(ancestorNode);

				// expand the ancestor
				// ReSharper disable LoopCanBeConvertedToQuery
				foreach (var item in QueryApplicableChildren(ancestorItem))
					expandedParents.Add(new ContentTreeNode(item, ancestorNode));
				// ReSharper restore LoopCanBeConvertedToQuery
			}


			// Add current item ================================================================

			if (currentPage.Visible || menuPart.MenuShowCurrentItemIfHidden)
			{
				// -- add a node for the current page --
				var navItemCurrent = new ContentTreeNode(currentPage, navTree.LastOrDefault());
				var navItemCParent = navTree.LastOrDefault();
				navTree.Add(navItemCurrent);

				// -- get children and siblings --
				// ReSharper disable LoopCanBeConvertedToQuery
				foreach (var child in QueryApplicableChildren(currentPage).OrderBy(f => f.SortOrder))
					navTree.Add(new ContentTreeNode(child, navItemCurrent));
				// ReSharper restore LoopCanBeConvertedToQuery
			}



			// add the ancestors we just expanded 
			// ReSharper disable LoopCanBeConvertedToQuery
			foreach (var item in expandedParents)
				if (navTree.All(f => f.ItemId != item.ItemId))
					navTree.Add(item);
			// ReSharper restore LoopCanBeConvertedToQuery



			// show siblings of the current item (put under navItemCParent)
			//if (menuPart.ShowSiblings != MenuPartBase.SiblingDisplayOptions.Never)
			//{
			//	if (menuPart.ShowSiblings == MenuPartBase.SiblingDisplayOptions.Always
			//		|| (menuPart.ShowSiblings == MenuPartBase.SiblingDisplayOptions.OnlyIfItemHasNoChildren && chil.Length > 0))
			//	{
			//		// ok...
			//		// ReSharper disable LoopCanBeConvertedToQuery
			//		foreach (var sibling in sibs.OrderBy(f => f.SortOrder))
			//			navTree.Add(new ContentTreeNode(sibling, navItemCParent));
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
#if ENABLE_DEBUG_COMMENTS
			xml.Write("<!-- WriteChildList(currentNode = {0}, level = {1} -->\n", currentNode.ItemId, level);
#endif
			xml.AddAttribute("class", currentNode == null ? menuPart.MenuOuterUlCssClass : menuPart.MenuInnerUlCssClass);
			xml.RenderBeginTag(HtmlTextWriterTag.Ul);

			if (currentNode == null && (!menuPart.MenuShowTreeRoot && childNodes.Count > 0)) // indicates that we are starting the menu, 
			{
				// Skip directly to the children of the root node.
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
				WriteListItem(childNode, xml, 0, "nav-header disabled"); // flattened header item

				var innerChildNodes = database.Where(f => f.Parent == childNode).ToList();
				foreach (var innerChildNode in innerChildNodes.OrderBy(n => n.SortOrder))
				{
					WriteListItem(innerChildNode, xml, 1, null);

					if (!menuPart.MenuNestChildUls)
					{
						WriteChildList(innerChildNode, xml, 2);
					}
				}
			}
			xml.RenderEndTag();
		}

		private void WriteListItem(ContentTreeNode childNode, HtmlTextWriter xml, int level, string cssClass)
		{
#if ENABLE_DEBUG_COMMENTS
			// debug output
			xml.Write("<!-- WriteListItem(childNode.itemId = {0}, parent.itemId = {2}, level = {1} -->", 
				childNode != null ? childNode.ItemId : -1, 
				level, 
				(childNode != null && childNode.Parent != null) ? childNode.Parent.ItemId : -1);
#endif

			// write LI...
			var sn = menuPart;
			var childItem = childNode.Item;
			var isSelected = childItem.ID == currentPageId;

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
					&& QueryApplicableChildren(childItem).Any() /* has any applicable children */)
				{
					if (!String.IsNullOrWhiteSpace(sn.MenuCaretCustomHtml))
						xml.Write(sn.MenuCaretCustomHtml);
					else
					{
						// <b class="caret"></b> 
						xml.Write(' ');
						xml.AddAttribute("class", "caret");
						xml.RenderBeginTag(HtmlTextWriterTag.B);
						xml.RenderEndTag();
					}
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
