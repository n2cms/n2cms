using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using N2.Templates.Faq.Items;
using N2.Resources;

namespace N2.Templates.Faq.UI
{
	public partial class Bubble : Templates.Web.UI.TemplateUserControl<ContentItem, BubbleItem>
	{
		protected override void OnInit(EventArgs e)
		{
			Register.StyleSheet(Page, "~/Faq/UI/Css/Faq.css");
			
			base.OnInit(e);
		}
	}
}