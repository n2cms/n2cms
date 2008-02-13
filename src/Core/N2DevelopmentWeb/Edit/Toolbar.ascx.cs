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
using System.Web.Compilation;
using N2.Definitions;

namespace N2.Edit
{
	public partial class Toolbar : System.Web.UI.UserControl
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			logout.ToolTip = string.Format(GetLocalResourceString("logout.ToolTipFormat"), Page.User.Identity.Name);
			LoadToolbarPlugIns();
		}

		private void LoadToolbarPlugIns()
		{
			foreach (ToolbarPlugInAttribute pluginAttribute in N2.Context.Instance.EditManager.GetToolbarPlugIns(this.Page.User))
			{
				Control container = (pluginAttribute.Area == ToolbarArea.Preview) 
					? plhFrame 
					: plhNavigation;
				pluginAttribute.AddTo(container, SelectedItem);
			}
		}

		#region PlugIn Methods

		private string CheckTranslation(ToolbarPlugInAttribute a, string possibleResourceKey)
		{
			if (possibleResourceKey != null)
			{
				if (!string.IsNullOrEmpty(a.GlobalResourceClassName))
					return GetGlobalResourceString(a.GlobalResourceClassName, possibleResourceKey);
				else return GetLocalResourceString(possibleResourceKey)
					?? possibleResourceKey;
			}
			return null;
		}
		#endregion

		#region Get Resource Methods
		protected string GetLocalResourceString(string resourceKey)
		{
			return (string)GetLocalResourceObject(resourceKey);
		}
		protected string GetGlobalResourceString(string className, string resourceKey)
		{
			return (string)GetGlobalResourceObject(className, resourceKey);
		}

		#endregion

		protected ContentItem SelectedItem
		{
			get { return ((N2.Edit.Web.EditPage)this.Page).SelectedItem; }
		}
	}
}