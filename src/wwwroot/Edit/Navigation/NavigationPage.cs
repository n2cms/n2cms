using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace N2.Edit.Navigation
{
	public class NavigationPage : Web.EditPage
	{
		protected string Path
		{
			get { return Request["root"] ?? "/"; }
		}

		protected ContentItem RootNode
		{
			get { return Engine.Resolve<Navigator>().Navigate(Path); }
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			RegisterClientSideScripts();
		}

		private void RegisterClientSideScripts()
		{
			string script = @"
jQuery(document).ready( function() {
	n2nav.setupLinks('#nav');
});";
			Page.ClientScript.RegisterStartupScript(typeof(NavigationPage), "NavigationPage.ClientScript", script, true);
			N2.Resources.Register.JQuery(this);
		}
	}
}
