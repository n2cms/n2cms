using System.IO;
using System.Web.UI;
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

		public abstract TitleDisplayOptions TitleDisplayMode { get; set; }
		public abstract HeadingLevel TitleHeadingLevel { get; set; }
		public abstract int NumChildLevels { get; set; }
		public abstract int StartFromLevel { get; set; }
		public abstract SibilingDisplayOptions ShowSibilings { get; set; }
		public abstract bool FlattenTopLevel { get; set; }
		public abstract bool DontLinkTopLevel { get; set; }
		public abstract bool ShowCaretOnItemsWithChildren { get; set; }
		public abstract string OuterUlCssClass { get; set; }
		public abstract string InnerUlCssClass { get; set; }
		public abstract string LiCssClass { get; set; }
		public abstract string SelectedLiCssClass { get; set; }
	}

	/// <summary>
	/// Provides navigation across child and sibiling pages. Ideal for sidebars. 
	/// </summary>
	[PartDefinition(Title = "Menu", IconUrl = "~/N2/Resources/icons/bullet.png")]
	[WithEditableTitle]
	[FieldSetContainer(NestingContainerName, "Hierarchy View Settings", 400)]
	[FieldSetContainer(CssContainerName, "Developer: Stylesheets", 500)]
	public class MenuPart : MenuPartBase, IPart
	{
		#region Title Display

		[EditableEnum("Title Display Mode", 100, typeof(TitleDisplayOptions))]
		public override TitleDisplayOptions TitleDisplayMode
		{
			get { return GetDetail("TitleDisplay", TitleDisplayOptions.None); }
			set { SetDetail("TitleDisplay", value); }
		}

		[EditableEnum("Title Heading Format", 150, typeof(HeadingLevel))]
		public override HeadingLevel TitleHeadingLevel
		{
			get { return GetDetail("TitleMode", HeadingLevel.H1); }
			set { SetDetail("TitleMode", value); }
		}

		#endregion 


		#region Hierarchy Options

		[EditableNumber("Num Child Levels", 400, MinimumValue = "0", Required = true, ContainerName = NestingContainerName)]
		public override int NumChildLevels
		{
			get { return GetDetail("Levels", 1); }
			set { SetDetail("Levels", value); }
		}

		[EditableNumber("Start from Level", 420, 
			Required = true,
			HelpText = "Positive values are absolute from root; negative values are relative-up from current item.",
			ContainerName = NestingContainerName)]
		public override int StartFromLevel
		{
			get { return GetDetail("StartLevel", 2); }
			set { SetDetail("StartLevel", value); }
		}


		[EditableEnum(typeof(SibilingDisplayOptions), Title = "Show Sibilings", ContainerName = NestingContainerName)]
		public override SibilingDisplayOptions ShowSibilings
		{
			get { return GetDetail("ShowSibilings",  SibilingDisplayOptions.OnlyIfItemHasNoChildren); }
			set { SetDetail("ShowSibilings", value); }
		}

		[EditableCheckBox("Flatten top-level items to list-headers", 460, ContainerName = NestingContainerName)]
		public override bool FlattenTopLevel
		{
			get { return GetDetail("ftl", false); }
			set { SetDetail("ftl", value); }
		}

		[EditableCheckBox("Do not link top-level items", 470, ContainerName = NestingContainerName)]
		public override bool DontLinkTopLevel
		{
			get { return GetDetail("nll", false); }
			set { SetDetail("nll", value); }
		}

		[EditableCheckBox("Show caret on items with children", 480, ContainerName = NestingContainerName)]
		public override bool ShowCaretOnItemsWithChildren
		{
			get { return GetDetail("sc", false); }
			set { SetDetail("sc", value); }
		}

		#endregion 


		#region CSS Overrides

		[EditableText("Outer UL CssClass", 500, ContainerName = CssContainerName)]
		public override string OuterUlCssClass
		{
			get { return GetDetail("OuterUlClass", "nav nav-list"); }
			set { SetDetail("OuterUlClass", value); }
		}

		[EditableText("Inner UL CssClass", 510, ContainerName = CssContainerName)]
		public override string InnerUlCssClass
		{
			get { return GetDetail("InnerUlClass", "nav nav-list nav-inner"); }
			set { SetDetail("InnerUlClass", value); }
		}

		[EditableText("Item CssClass", 520, ContainerName = CssContainerName)]
		public override string LiCssClass
		{
			get { return GetDetail("LiCssClass", "nav-item"); }
			set { SetDetail("LiCssClass", value); }
		}

		[EditableText("Selected Item CssClass", 530, ContainerName = CssContainerName)]
		public override string SelectedLiCssClass
		{
			get { return GetDetail("SelectedLiCssClass", "nav-item active"); }
			set { SetDetail("SelectedLiCssClass", value); }
		}

		#endregion

	}

	/// <summary>
	/// Renders the SubNavigation part using the ASP.NET MVC framework.
	/// </summary>
	[Adapts(typeof (MenuPart))]
	public class MenuPartMvcAdapter: MvcAdapter
	{
		public override void RenderTemplate(System.Web.Mvc.HtmlHelper html, ContentItem model)
		{
			if (!(model is MenuPart))
				throw new ArgumentException("model");

			if (html.ViewContext.Writer is HtmlTextWriter)
				(new MenuPartRenderer(model as MenuPart)).WriteHtml(html.ViewContext.Writer as HtmlTextWriter);
			else
				html.ViewContext.Writer.Write(new MenuPartRenderer(model as MenuPart).GetHtml());
		}
	}

	//TODO: Write a WebForms compatible adapter for MenuPart.


	
	public sealed class MenuPartRenderer
	{

		private class ContentTreeNode
		{
			public ContentTreeNode(ContentItem item, ContentTreeNode parent)
			{
				Item = item;
				Parent = parent;
				SortOrder = item.SortOrder;
			}

			public int SortOrder { get; protected set; }
			public ContentItem Item { get; private set; }
			public ContentTreeNode Parent { get; private set; }
		}

		private readonly List<ContentTreeNode> database;
		private readonly MenuPartBase menuPart;
		private int cId;

		public MenuPartRenderer(MenuPartBase menuPart)
		{
			this.menuPart = menuPart;
			database = BuildNavTree();
		}

		public string GetHtml()
		{

			StringBuilder sb = new StringBuilder();
			using (var sw = new StringWriter(sb))
			using (var xml = new XhtmlTextWriter(sw))
				WriteHtml(xml);
			return sb.ToString();
		}


		public void WriteHtml(HtmlTextWriter xml)
		{
			xml.AddAttribute("id", String.IsNullOrEmpty(menuPart.Name) ? "menu" + menuPart.ID : menuPart.Name);
			xml.RenderBeginTag("div");

			switch (menuPart.TitleDisplayMode)
			{
				case MenuPart.TitleDisplayOptions.CustomTitle:
					xml.WriteLine(HeadingLevelUtility.DoTitle(menuPart.TitleHeadingLevel, menuPart.Title));
					break;
				case MenuPart.TitleDisplayOptions.CurrentPageTitle:
					xml.WriteLine(HeadingLevelUtility.DoTitle(menuPart.TitleHeadingLevel, Content.Current.Page.Title));
					break;
				case MenuPart.TitleDisplayOptions.None:
					break;
			}

			if (menuPart.FlattenTopLevel)
			{
				WriteListWithHeaders(null, xml);
			}
			else
			{
				WriteChildList(null, xml, 0);
			}

			xml.RenderEndTag(); // </div>
		}

		private List<ContentTreeNode> BuildNavTree()
		{
			var ci = Content.Current.Page;
			cId = ci.ID; // need to cache this due to the following if clause:
			if (ci.VersionOf != null && ci.VersionOf.Value != null)
				ci = ci.VersionOf.Value; // get the published version

			List<ContentTreeNode> navTree = new List<ContentTreeNode>();
			{
				// follow the ancestral trail up to the desired "start from level"
				var convertedAncestralTrail = Array.ConvertAll(
					ci.AncestralTrail.Split(new char[] {'/'}, StringSplitOptions.RemoveEmptyEntries),
					int.Parse);

				var xn = menuPart.StartFromLevel;
				if (xn < 0)
					xn += convertedAncestralTrail.Length; // handle "zero" case

				for (var i = Math.Max(xn, 0); i < convertedAncestralTrail.Length; ++i)
				{
					var ancestorItem = Context.Current.Persister.Get(Convert.ToInt32(convertedAncestralTrail[i]));
					var ancestorNode = new ContentTreeNode(ancestorItem, navTree.LastOrDefault());
					navTree.Add(ancestorNode);
				}

				// add a node for the current page
				var navItemCurrent = new ContentTreeNode(ci, navTree.LastOrDefault());
				var navItemCParent = navTree.LastOrDefault();
				navTree.Add(navItemCurrent);

				// get children and sibilings
				var sibs = (from x in ci.Parent.GetChildren()
				            where x.ID != cId && x.IsPage && x.IsPublished()
				            select x).ToArray();

				var chil = (from x in ci.GetChildren() 
							where x.IsPage && x.IsPublished()
							select x).ToArray();

				// show sibilings of the current item (put under navItemCParent)
				if (menuPart.ShowSibilings != MenuPartBase.SibilingDisplayOptions.Never)
				{
					if (menuPart.ShowSibilings == MenuPartBase.SibilingDisplayOptions.Always
					    || (menuPart.ShowSibilings == MenuPartBase.SibilingDisplayOptions.OnlyIfItemHasNoChildren && chil.Length > 0))
					{
						// ok...
						// ReSharper disable LoopCanBeConvertedToQuery
						foreach (var sibiling in sibs.OrderBy(f => f.SortOrder))
							navTree.Add(new ContentTreeNode(sibiling, navItemCParent));
						// ReSharper restore LoopCanBeConvertedToQuery
					}
				}

				// show children of the current item  (put under navItemCurrent)
				// ReSharper disable LoopCanBeConvertedToQuery
				foreach (var child in chil.OrderBy(f => f.SortOrder))
					navTree.Add(new ContentTreeNode(child, navItemCurrent));
				// ReSharper restore LoopCanBeConvertedToQuery
			}
			return navTree;
		}


		// ReSharper disable ParameterTypeCanBeEnumerable.Local
		private void WriteChildList(ContentTreeNode currentNode, HtmlTextWriter xml, int level)
		{
			var childNodes = database.Where(f => f.Parent == currentNode).ToList();
			if (childNodes.Count <= 0) return;

			xml.AddAttribute("class", currentNode == null ? menuPart.OuterUlCssClass : menuPart.InnerUlCssClass);
			xml.RenderBeginTag(HtmlTextWriterTag.Ul);
			foreach (var childNode in childNodes)
			{
				WriteListItem(childNode, xml, level, null);
				WriteChildList(childNode, xml, level + 1);
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

			xml.AddAttribute("class", currentNode == null ? menuPart.OuterUlCssClass : menuPart.InnerUlCssClass);
			xml.RenderBeginTag(HtmlTextWriterTag.Ul);
			foreach (var childNode in childNodes)
			{
				WriteListItem(childNode, xml, 0, "nav-header"); // header item

				var childNodes2 = database.Where(f => f.Parent == childNode).ToList();
				foreach (var childnode2 in childNodes2)
				{
					WriteListItem(childnode2, xml, 1, null);
					WriteChildList(childnode2, xml, 2);
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
				cssClass = isSelected ? sn.SelectedLiCssClass : sn.LiCssClass;
			xml.AddAttribute("class", cssClass);
			xml.RenderBeginTag(HtmlTextWriterTag.Li);

			if (isSelected || (level == 0 && sn.DontLinkTopLevel))
			{
				xml.Write(childItem.Title);
			}
			else
			{
				xml.AddAttribute("href", childItem.Url);
				xml.RenderBeginTag(HtmlTextWriterTag.A);
				xml.Write(childItem.Title);

				// render caret if subitems exist
				if (sn.ShowCaretOnItemsWithChildren
					&& database.Any(f => f.Parent == childNode))
				{
					// <b class="caret"></b> 
					xml.Write(' ');
					xml.AddAttribute("class", "caret");
					xml.RenderBeginTag(HtmlTextWriterTag.B);
					xml.RenderEndTag();
				}
				xml.RenderEndTag();
			}

			xml.RenderEndTag(); // </li>
			xml.WriteLine();
		}

	}
	
}