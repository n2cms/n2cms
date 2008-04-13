using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using N2.Integrity;

namespace N2.Templates.Faq.Items
{
	[Definition]
	[AllowedZones(Zones.Left, Zones.Right)]
	public class BubbleItem : Templates.Items.AbstractItem
	{
		[N2.Details.EditableFreeTextArea("Text", 100)]
		public virtual string Text
		{
			get { return (string)(GetDetail("Text") ?? string.Empty); }
			set { SetDetail("Text", value, string.Empty); }
		}

		public override string IconUrl
		{
			get { return "~/Faq/UI/Img/help.png"; }
		}

		public override string TemplateUrl
		{
			get { return "~/Faq/UI/Bubble.ascx"; }
		}
	}
}
