using System;
using N2.Templates.Web.UI;

namespace N2.Addons.UITests.UI
{
	public partial class ItemEditor : TemplatePage<Items.AdaptiveItemPage>
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			ie.CurrentItem = CurrentPage;
		}

		protected void Save_Click(object sender, EventArgs e)
		{
			ie.Save();
			Response.Redirect(CurrentPage.Url);
		}
	}
}
