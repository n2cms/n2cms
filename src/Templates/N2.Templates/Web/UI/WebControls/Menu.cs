using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using N2.Collections;
using N2.Web.UI;

namespace N2.Templates.Web.UI.WebControls
{
	public class Menu : ItemAwareControl, IContentTemplate
	{
		private ItemFilter[] filters = null;

		private ContentItem startPage;

		public ContentItem StartPage
		{
			get { return startPage ?? Find.ClosestStartPage; }
			set { startPage = value; }
		}

		public int StartLevel
		{
			get { return (int)(ViewState["StartLevel"] ?? 1); }
			set { ViewState["StartLevel"] = value; }
		}

		public bool BranchMode
		{
			get { return (bool)(ViewState["BranchMode"] ?? true); }
			set { ViewState["BranchMode"] = value; }
		}

		public int MaxLevels
		{
			get { return (int)(ViewState["MaxLevels"] ?? int.MaxValue); }
			set { ViewState["MaxLevels"] = value; }
		}

		public string CssClass
		{
			get { return (string)(ViewState["CssClass"] ?? string.Empty); }
			set { ViewState["CssClass"] = value; }
		}

		public Menu()
		{
			Filters = new ItemFilter[] {new NavigationFilter()};
		}

		#region Methods

		protected override void CreateChildControls()
		{
			Control ul = BuildControlHierarchy(CurrentPage, StartPage);
			if (ul != null)
				Controls.Add(ul);

			base.CreateChildControls();
		}
		
		private static HtmlGenericControl CreateAndAdd(Control container, string tagName, string cssClass)
		{
			HtmlGenericControl hgc = new HtmlGenericControl(tagName);
			if(!string.IsNullOrEmpty(cssClass))
				hgc.Attributes["class"] = cssClass;
			container.Controls.Add(hgc);
			return hgc;
		}

		private Control AddControlsRecursive(Control container, IHierarchyNavigator<ContentItem> ih, ContentItem selectedPage, IEnumerable<ContentItem> ancestors)
		{
			HtmlGenericControl ul = null;
			foreach (ItemHierarchyNavigator childHierarchy in ih.Children)
			{
				if (!childHierarchy.Current.IsPage)
					continue;

				if (ul == null)
				{
					ul = CreateAndAdd(container, "ul", CssClass);
				}

				HtmlGenericControl li = CreateAndAdd(ul, "li", null);

				ContentItem current = childHierarchy.Current;
				li.Controls.Add(N2.Web.Link.To(current).ToControl());

				if (current == selectedPage || current.Url == selectedPage.Url)
					li.Attributes["class"] = "current";
				else if (Contains(ancestors, current))
					li.Attributes["class"] = "trail";

				AddControlsRecursive(li, childHierarchy, selectedPage, ancestors);
			}
			return ul;
		}

		private static bool Contains(IEnumerable<ContentItem> ancestors, ContentItem item)
		{
			foreach (ContentItem ancestor in ancestors)
				if (ancestor == item)
					return true;
			return false;
		}
		
		private Control BuildControlHierarchy(ContentItem currentItem, ContentItem startPage)
		{
			ItemList children = currentItem.GetChildren();
			if (children.Count > 0)
				currentItem = children[0];
			IEnumerable<ContentItem> ancestors = GetAncestors(currentItem, startPage);
			ContentItem startingPoint = GetStartingPoint();
			if(startingPoint != null)
			{
				ItemHierarchyNavigator ih;
				if (BranchMode)
				{
					ih = new ItemHierarchyNavigator(new BranchHierarchyBuilder(currentItem, startingPoint), Filters);
				}
				else
				{
					ih = new ItemHierarchyNavigator(new TreeHierarchyBuilder(startingPoint, MaxLevels), Filters);
				}
				if(ih.Current != null)
					return AddControlsRecursive(this, ih, CurrentPage, ancestors);
			}
			return null;
		}

		private ItemFilter[] Filters
		{
			get { return filters; }
			set { filters = value; }
		}

		private ContentItem GetStartingPoint()
		{
			return Find.FindAncestorAtLevel(StartLevel, Find.EnumerateParents(CurrentPage, StartPage, true), CurrentPage);
		}

		private static IEnumerable<ContentItem> GetAncestors(ContentItem currentItem, ContentItem startPage)
		{
			return Find.EnumerateParents(currentItem, startPage);
		}

		#endregion [rgn]

		#region IPageItemContainer Members

		public ContentItem CurrentPage
		{
			get { return Find.CurrentPage; }
		}

		#endregion
	}
}
