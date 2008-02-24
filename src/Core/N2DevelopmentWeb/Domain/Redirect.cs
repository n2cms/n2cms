using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using N2.Details;
using N2;

namespace N2DevelopmentWeb.Domain
{
	[Definition]
	[WithEditableTitle]
	[WithEditableName]
	public class Redirect : N2.ContentItem
	{
		[EditableUrl("RedirectUrl", 100)]
		public virtual string RedirectUrl
		{
			get { return (string)(GetDetail("RedirectUrl") ?? string.Empty); }
			set { SetDetail("RedirectUrl", value, string.Empty); }
		}

		public override string Url
		{
			get { return RedirectUrl; }
		}
	}
}
