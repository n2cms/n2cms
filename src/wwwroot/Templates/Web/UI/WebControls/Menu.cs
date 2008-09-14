using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using N2.Collections;
using N2.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Templates.Web.UI.WebControls
{
	/// <summary>
	/// A web control that emits a nested list of ul's and li's.
	/// </summary>
	public class Menu : WebControl, IContentTemplate
	{
		private ItemFilter[] filters = null;
		private ContentItem startPage;
		private ContentItem currentItem = null;

		public Menu()
		{
			Filters = new ItemFilter[] { new NavigationFilter() };
		}

		public virtual ContentItem CurrentItem
		{
			get { return currentItem ?? (currentItem = ItemUtility.FindCurrentItem(Parent)); }
			set { currentItem = value; }
		}

		public ContentItem StartPage
		{
			get { return startPage ?? Find.ClosestLanguageRoot; }
			set { startPage = value; }
		}

		[Themeable(true)]
		public int StartLevel
		{
			get { return (int)(ViewState["StartLevel"] ?? 1); }
			set { ViewState["StartLevel"] = value; }
		}

		[Themeable(true)]
		public bool BranchMode
		{
			get { return (bool)(ViewState["BranchMode"] ?? true); }
			set { ViewState["BranchMode"] = value; }
		}

		[Themeable(true)]
		public int MaxLevels
		{
			get { return (int)(ViewState["MaxLevels"] ?? int.MaxValue); }
			set { ViewState["MaxLevels"] = value; }
		}

		protected override HtmlTextWriterTag TagKey
		{
			get { return HtmlTextWriterTag.Ul; }
		}

		#region Methods

		protected override void CreateChildControls()
		{
			BuildControlHierarchy(CurrentPage, StartPage);
			
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

		private static bool Contains(IEnumerable<ContentItem> ancestors, ContentItem item)
		{
			foreach (ContentItem ancestor in ancestors)
				if (ancestor == item)
					return true;
			return false;
		}
		
		private void BuildControlHierarchy(ContentItem currentItem, ContentItem startPage)
		{
			ItemList children = currentItem.GetChildren();
			if (children.Count > 0)
				currentItem = children[0];
			IEnumerable<ContentItem> ancestors = GetAncestors(currentItem, startPage);
			ContentItem startingPoint = GetStartingPoint();
			if(startingPoint != null)
			{
				ItemHierarchyNavigator navigator;
				if (BranchMode)
				{
					navigator = new ItemHierarchyNavigator(new BranchHierarchyBuilder(currentItem, startingPoint), Filters);
				}
				else
				{
					navigator = new ItemHierarchyNavigator(new TreeHierarchyBuilder(startingPoint, MaxLevels), Filters);
				}
				if (navigator.Current != null)
				{
					AddControlsRecursive(this, navigator, CurrentPage, ancestors);
				}
			}
		}

		private void AddControlsRecursive(Control container, IHierarchyNavigator<ContentItem> ih, ContentItem selectedPage, IEnumerable<ContentItem> ancestors)
		{
			foreach (ItemHierarchyNavigator childHierarchy in ih.Children)
			{
				if (!childHierarchy.Current.IsPage)
					continue;

				HtmlGenericControl li = CreateAndAdd(container, "li", null);

				ContentItem current = childHierarchy.Current;
				li.Controls.Add(N2.Web.Link.To(current).ToControl());

				if (current == selectedPage || current.Url == selectedPage.Url)
					li.Attributes["class"] = "current";
				else if (Contains(ancestors, current))
					li.Attributes["class"] = "trail";

				HtmlGenericControl ul = new HtmlGenericControl("ul");
				AddControlsRecursive(ul, childHierarchy, selectedPage, ancestors);
				if (ul.Controls.Count > 0)
					li.Controls.Add(ul);
			}
		}

		private ItemFilter[] Filters
		{
			get { return filters; }
			set { filters = value; }
		}

		private ContentItem GetStartingPoint()
		{
			return Find.AncestorAtLevel(StartLevel, Find.EnumerateParents(CurrentPage, StartPage, true), CurrentPage);
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
