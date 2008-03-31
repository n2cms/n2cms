using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace N2.Edit
{
    public partial class AffectedItems : System.Web.UI.UserControl, N2.Web.UI.IDataItemContainer
    {
        protected override void OnDataBinding(EventArgs e)
        {
            base.OnPreRender(e);

            this.Controls.Clear();
            HtmlGenericControl ul = new HtmlGenericControl("ul");
            this.Controls.Add(ul);
            AddChildrenRecursive(CurrentItem, ul);
        }

        private void AddChildrenRecursive(N2.ContentItem item, HtmlGenericControl container)
        {
            HtmlGenericControl li = new HtmlGenericControl("li");
            li.InnerHtml = string.Format("<a href='{0}'><img src='{1}'/>{2}</a>", item.Url, VirtualPathUtility.ToAbsolute(item.IconUrl), item.Title);
            container.Controls.Add(li);

            if (item.Children.Count > 0)
            {
                HtmlGenericControl ul = new HtmlGenericControl("ul");
                li.Controls.Add(ul);

                foreach (N2.ContentItem child in item.Children)
                    AddChildrenRecursive(child, ul);
            }
		}

		private ContentItem currentData;
		public N2.ContentItem CurrentItem
		{
			get { return currentData; }
			set { currentData = value; }
		}
	}
}