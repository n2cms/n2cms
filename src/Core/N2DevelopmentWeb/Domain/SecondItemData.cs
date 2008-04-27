using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace N2DevelopmentWeb.Domain
{
	[N2.Integrity.AllowedZones("Right")]
	public class SecondItemData : AbstractCustomItem
	{
		public override bool IsPage
		{
			get { return false; }
		}
	}
}
