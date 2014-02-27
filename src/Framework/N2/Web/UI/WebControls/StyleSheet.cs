using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace N2.Web.UI.WebControls
{
    public class StyleSheet : Control
    {
        private string cssUrl;
        public string CssUrl 
        {
            get { return cssUrl; }
            set { cssUrl = value; }
        }

        protected override void OnPreRender(EventArgs e)
        {
            HtmlLink link = new HtmlLink();
            link.Attributes["type"] = "text/css";
            link.Attributes["rel"] = "stylesheet";
            link.Href = CssUrl;

            if (Page.Header != null)
                Page.Header.Controls.Add(link);
            else
                Controls.Add(link);

            base.OnPreRender(e);
        }
    }
}
