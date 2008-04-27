using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using N2.Integrity;
using N2DevelopmentWeb.Domain;

namespace N2DevelopmentWeb.Domain
{
	[AllowedZones("Left", "Right")]
	public class ItemCreator : MyItemData
	{
		public override string TemplateUrl
		{
			get
			{
				return "~/Uc/ItemCreatorUI.ascx";
			}
		}
	}
}
