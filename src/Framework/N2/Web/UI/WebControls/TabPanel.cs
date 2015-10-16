using System;
using System.Web.UI.WebControls;
using N2.Resources;

namespace N2.Web.UI.WebControls
{
    /// <summary>
    /// A panel that is made into a tab panel throgh jquery client side 
    /// scripting.
    /// </summary>
    public class TabPanel : Panel
    {
        public TabPanel()
        {
            CssClass = "tabPanel";
        }

        public bool RegisterTabCss
        {
            get { return (bool)(ViewState["RegisterTabCss"] ?? false); }
            set { ViewState["RegisterTabCss"] = value; }
        }

        /// <summary>The tab caption.</summary>
        public string TabText { get; set; }

        /// <summary>Renders a link on the tab.</summary>
        public string NavigateUrl { get; set; }

        /// <summary>Displays this tab when the page is loaded.</summary>
        public bool Selected { get; set; }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            Visible = Controls.Count > 0;
            if(Visible)
				Register.TabPanel(Page, ".tabPanel", RegisterTabCss);
        }

        protected override void AddAttributesToRender(System.Web.UI.HtmlTextWriter writer)
        {
            if (!string.IsNullOrEmpty(NavigateUrl))
                writer.AddAttribute("data-tab-href", Page.ResolveClientUrl(NavigateUrl));
            if (!string.IsNullOrEmpty(TabText))
                writer.AddAttribute("data-tab-text", TabText);
            if (Selected)
                writer.AddAttribute("data-tab-selected", "true");

            base.AddAttributesToRender(writer);
        }
    }
}
