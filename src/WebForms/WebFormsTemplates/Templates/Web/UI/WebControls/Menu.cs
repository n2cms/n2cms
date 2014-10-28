using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using N2.Collections;
using N2.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Web;

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
        Dictionary<string, Control> addedControls = new Dictionary<string, Control>();

        public Menu()
        {
            Filters = new ItemFilter[] { new NavigationFilter() };
        }

        public virtual ContentItem CurrentItem
        {
            get { return currentItem ?? (currentItem = (Site != null && Site.DesignMode) ? null : Find.ClosestPage(NamingContainer)); }
            set { currentItem = value; }
        }
        
        public ContentItem StartPage
        {
            get { return startPage ?? (startPage = (Site != null && Site.DesignMode) ? null : Find.ClosestLanguageRoot); }
            set { startPage = value; }
        }

        [Themeable(true)]
        public bool UseMenuIdentifiers
        {
            get { return (bool)(ViewState["UseMenuIdentifiers"] ?? true); }
            set { ViewState["UseMenuIdentifiers"] = value; }
        }

        [Themeable(true)]
        public string MenuIdentifierPrefix
        {
            get { return (string)(ViewState["MenuIdentifierPrefix"] ?? "menu_"); }
            set { ViewState["MenuIdentifierPrefix"] = value; }
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

        private ItemFilter[] Filters
        {
            get { return filters; }
            set { filters = value; }
        }

        protected override HtmlTextWriterTag TagKey
        {
            get { return HtmlTextWriterTag.Ul; }
        }

        #region Methods

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            if (Site != null && Site.DesignMode)
                return;

            BuildControlHierarchy(CurrentItem, StartPage);
        }

        private static HtmlGenericControl CreateAndAdd(Control container, string tagName, string cssClass)
        {
            HtmlGenericControl hgc = new HtmlGenericControl(tagName);
            if (!string.IsNullOrEmpty(cssClass))
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
            if (currentItem == null)
                currentItem = startPage;

            var children = currentItem.Children.WhereNavigatable();
            if (children.Count > 0)
                currentItem = children[0];
            IEnumerable<ContentItem> ancestors = GetAncestors(currentItem, startPage);
            ContentItem startingPoint = GetStartingPoint();
            if (startingPoint != null)
            {
                HierarchyBuilder builder = null;

                if (BranchMode)
                {
                    builder = new BranchHierarchyBuilder(currentItem, startingPoint);
                }
                else
                {
                    builder = new TreeHierarchyBuilder(startingPoint, MaxLevels);
                }

                HierarchyNode<ContentItem> node = builder.Children(Filters).Build();
                if (node.Current != null)
                {
                    AddControlsRecursive(this, node, CurrentItem, ancestors);
                }
            }
        }

        private void AddControlsRecursive(Control container, HierarchyNode<ContentItem> ih, ContentItem selectedPage, IEnumerable<ContentItem> ancestors)
        {
            foreach (HierarchyNode<ContentItem> childHierarchy in ih.Children)
            {
                if (!childHierarchy.Current.IsPage)
                    continue;

                ContentItem current = childHierarchy.Current;

                string _cssClass =
                    current == selectedPage
                    || string.Equals(current.Url, selectedPage.Url, StringComparison.OrdinalIgnoreCase)
                        ? "current"
                        : Contains(ancestors, current)
                            ? "trail"
                            : string.Empty;

                HtmlGenericControl li = CreateAndAdd(container, "li", _cssClass);

                if (UseMenuIdentifiers)
                {
                    string name = Regex.Replace(current.Name, "[^_0-1a-z]", "", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    if (!addedControls.ContainsKey(name))
                    {
                        addedControls[current.Name] = li;
                        li.Attributes["id"] = MenuIdentifierPrefix + name;
                    }
                }

                li.Controls.Add(N2.Web.Link.To(current).ToControl());

                HtmlGenericControl ul = new HtmlGenericControl("ul");
                AddControlsRecursive(ul, childHierarchy, selectedPage, ancestors);
                if (ul.Controls.Count > 0)
                    li.Controls.Add(ul);
            }
        }

        private ContentItem GetStartingPoint()
        {
            return Find.AncestorAtLevel(StartLevel, Find.EnumerateParents(CurrentItem, StartPage, true), CurrentItem);
        }

        private static IEnumerable<ContentItem> GetAncestors(ContentItem currentItem, ContentItem startPage)
        {
            return Find.EnumerateParents(currentItem, startPage);
        }

        #endregion [rgn]
    }
}
