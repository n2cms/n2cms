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

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			Visible = Controls.Count > 0;
			if(Visible)
				Register.TabPanel(Page, "." + CssClass.Replace(' ', '.'), RegisterTabCss);
		}
	}
}