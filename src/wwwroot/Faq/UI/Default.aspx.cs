using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace N2.Templates.Faq.UI
{
	public partial class Default : Web.UI.TemplatePage<Items.FaqList>
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			Resources.Register.StyleSheet(this, "~/Faq/UI/Css/Faq.css", N2.Resources.Media.All);
		}
	}
}
