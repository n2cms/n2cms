using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using N2;
using N2.Web;

namespace N2.Edit.Web.UI.Controls
{
    public class ContentLink : HyperLink
    {
        public object DataSource { get; set; }
        public ContentItem CurrentItem { get; set; }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var item = CurrentItem ?? (DataSource as ContentItem);
            if (item != null)
            {
                NavigateUrl = item.Url;
                Text = string.Format("<img src='{0}'/> {1}", Url.ToAbsolute(item.IconUrl), item.Title);
            }
            this.Visible = item != null;
        }
    }
}
