using System;
using System.Web.UI.WebControls;
using N2.Resources;

namespace N2.Web.UI.WebControls
{
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

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			Register.TabPanel(Page, ".tabPanel", RegisterTabCss);
		}
	}
}