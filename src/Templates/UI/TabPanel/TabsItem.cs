using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using N2.Templates.UI.Items.Parts;

namespace N2.Templates.UI.TabPanel
{
	[Definition]
	public class TabsItem : TextItem
	{
		public override string TemplateUrl
		{
			get { return "~/TabPanel/TabsInterface.ascx"; }
		}

		public override string IconUrl
		{
			get { return "~/Img/tab.png"; }
		}
	}
}
