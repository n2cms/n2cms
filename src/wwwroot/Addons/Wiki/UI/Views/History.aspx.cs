using System;

namespace N2.Addons.Wiki.UI.Views
{
	public partial class History : WikiTemplatePage
	{
		protected override void OnInit(EventArgs e)
		{
			rptArticles.DataSource = N2.Find.Items
				.Where.VersionOf.Eq(CurrentPage)
				.Select();
			DataBind();
			base.OnInit(e);
		}
	}
}