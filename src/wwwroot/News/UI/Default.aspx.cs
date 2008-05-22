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

namespace N2.Templates.News.UI
{
	public partial class Default : Web.UI.TemplatePage<Items.News>
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			Resources.Register.StyleSheet(this, "~/News/UI/Css/News.css", N2.Resources.Media.All);
		}
	}
}
