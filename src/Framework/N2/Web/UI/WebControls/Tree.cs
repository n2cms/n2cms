using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using N2.Collections;
using N2.Edit;

namespace N2.Web.UI.WebControls
{
    public class Tree : Control, IItemContainer
    {
        private ContentItem currentItem;
        private ContentItem rootItem;
        private string target = Targets.Preview;
        private IList<string> icons = new List<string>();

        public Tree()
        {
            rootItem = Find.RootItem;
        }

        public ContentItem CurrentItem
        {
            get { return currentItem; }
            set { currentItem = value; }
        }

        public ContentItem RootItem
        {
            get { return rootItem ?? Find.RootItem; }
            set { rootItem = value; }
        }

        public string Target
        {
            get { return target; }
            set { target = value; }
        }

        public override void DataBind()
        {
            base.DataBind();
            EnsureChildControls();
        }

        protected override void CreateChildControls()
        {
            ItemFilter[] filters = GetFilters();
            Web.Tree t = N2.Web.Tree.From(RootItem)
                .OpenTo(CurrentItem).Filters(filters)
                .LinkWriter((n, w) => BuildLink(n.Current, CurrentItem, Target).WriteTo(w));
            Control ul = t.ToControl();
            Controls.Add(ul);

            for (int i = 0; i < icons.Count; i++)
            {
                Page.ClientScript.RegisterArrayDeclaration("icons", string.Format("'{0}'", icons[i]));
            }

            base.CreateChildControls();
        }

        private ItemFilter[] GetFilters()
        {
            bool displayDataItems = N2.Context.Current.Resolve<Edit.Settings.NavigationSettings>().DisplayDataItems;
            return displayDataItems
                    ? new ItemFilter[] { new AccessFilter(Page.User, N2.Context.SecurityManager) }
                    : new ItemFilter[] { new PageFilter(), new AccessFilter(Page.User, N2.Context.SecurityManager) };
        }

        private ILinkBuilder BuildLink(ContentItem item, ContentItem selectedItem, string target)
        {
            StringBuilder className = new StringBuilder();

            if (!item.Published.HasValue || item.Published > N2.Utility.CurrentTime())
                className.Append("unpublished ");
            else if (item.Published > N2.Utility.CurrentTime().AddDays(-2))
                className.Append("new ");

            if (item.Expires.HasValue && item.Expires <= N2.Utility.CurrentTime())
                className.Append("expired ");

            if (item == selectedItem)
                className.Append("selected ");

            if (item.AuthorizedRoles != null && item.AuthorizedRoles.Count > 0)
                className.Append("locked ");

            string iconUrl = item.IconUrl;
            int iconIndex = icons.IndexOf(iconUrl);
            if(iconIndex<0)
            {
                iconIndex = icons.Count;
                icons.Add(iconUrl);
            }
            className.Append("i" + iconIndex + " ");

            ILinkBuilder builder = Link.To(item).Target(target).Href(item.Url);
            if (className.Length > 0)
            {
                --className.Length; // remove trailing whitespace
                builder.Class(className.ToString());
            }
            
            return builder;
        }
    }
}
