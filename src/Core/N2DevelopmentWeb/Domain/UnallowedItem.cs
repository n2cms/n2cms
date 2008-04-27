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
	[N2.Definition("Unallowed item", "UnallowedItem")]
	[N2.Definitions.ItemAuthorizedRoles("NonExistantUserOrGroup")]
	public class UnallowedItem : AbstractCustomItem
	{
	}
}
