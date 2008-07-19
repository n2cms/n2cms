using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2;
using N2.Resources;

namespace N2.Templates.UI.TabPanel
{
	public partial class TabsInterface : N2.Web.UI.ContentUserControl<ContentItem, TabsItem>
	{
		protected override void OnInit(EventArgs e)
		{
			Register.JQuery(Page);
			Register.StyleSheet(Page, "~/TabPanel/TabPanel.css");
			base.OnInit(e);
		}
	}
}