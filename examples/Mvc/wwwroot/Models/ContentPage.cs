using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using N2;
using N2.Details;

namespace MvcTest.Models
{
	[Definition("Content Page", "Test", Installer = N2.Installation.InstallerHint.PreferredStartPage)]
	public class ContentPage : AbstractPage
	{
		[EditableFreeTextArea("Text", 100)]
		public virtual string Text
		{
			get { return (string)(GetDetail("Text") ?? string.Empty); }
			set { SetDetail("Text", value, string.Empty); }
		}

		public override string TemplateUrl
		{
			get { return "~/Views/Content/DefaultView.aspx"; }
		}
	}
}
