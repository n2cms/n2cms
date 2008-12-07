using System;

namespace N2.Edit.Navigation
{
	public class NavigationPage : Web.EditPage
	{
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
