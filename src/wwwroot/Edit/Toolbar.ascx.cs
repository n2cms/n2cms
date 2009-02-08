using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using N2.Edit.Web;
using N2.Web.UI.WebControls;

namespace N2.Edit
{
	public partial class Toolbar : EditUserControl
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			logout.ToolTip = string.Format(GetLocalResourceString("logout.ToolTipFormat"), Page.User.Identity.Name);
			LoadToolbarPlugIns();
		}

		private void LoadToolbarPlugIns()
		{
			foreach (ToolbarPluginAttribute plugin in N2.Context.Current.EditManager.GetPlugins<ToolbarPluginAttribute>(this.Page.User))
			{
				Control container = (plugin.Area == ToolbarArea.Preview) 
					? plhFrame 
					: plhNavigation;
				
				HtmlGenericControl command = new HtmlGenericControl("div");
				command.Attributes["id"] = plugin.Name;
				command.Attributes["class"] = "toolbarItem";
				container.Controls.Add(command);

				plugin.AddTo(command, new PluginContext(SelectedItem, ControlPanelState.Visible));
			}
		}

		#region PlugIn Methods

		private string CheckTranslation(ToolbarPluginAttribute a, string possibleResourceKey)
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
	}
}