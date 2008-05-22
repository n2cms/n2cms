using System;
using System.Web.UI.WebControls;
using N2.Resources;
using N2.Templates.Web.UI;

namespace N2.Templates.Calendar.UI
{
	public partial class Container : TemplatePage<Items.Calendar>
	{
		protected Repeater rc;

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			Register.StyleSheet(Page, "/Calendar/UI/Css/Calendar.css", Media.All);

			rc.DataSource = CurrentPage.GetEvents();
			rc.DataBind();
		}
	}
}
