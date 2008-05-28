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

namespace N2.Templates.ImageGallery.UI
{
	public partial class Default : Web.UI.TemplatePage<Items.ImageGallery>
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			N2.Resources.Register.JQuery(this);
		}
	}
}
