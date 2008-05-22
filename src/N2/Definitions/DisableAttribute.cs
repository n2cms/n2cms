using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using N2.Definitions;
using System.Collections.Generic;

namespace N2.Templates.UI.Items.LayoutParts
{
	/// <summary>
	/// Disables a definition removing it from lists when choosing new items. 
	/// Existing items will not be affaceted.
	/// </summary>
	public class DisableAttribute : Attribute, IDefinitionRefiner
	{
		public void Refine(ItemDefinition currentDefinition, IList<ItemDefinition> allDefinitions)
		{
			currentDefinition.Enabled = false;
		}
	}
}
