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

namespace N2.Edit.Security
{
	[N2.Edit.ToolbarPlugIn("", "security", "~/Edit/Security/Default.aspx?selected={selected}", ToolbarArea.Preview, "preview", "~/Edit/Img/Ico/lock.gif", 100, ToolTip = "allowed roles for selected item")]
	public partial class Default : Web.EditPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			hlCancel.NavigateUrl = SelectedItem.Url;
		}

		protected void cblAllowedRoles_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		protected void btnSave_Command(object sender, CommandEventArgs e)
		{
			if (AllSelected(cblAllowedRoles))
			{
				SelectedItem.AuthorizedRoles.Clear();
			}
			else
			{
				foreach (ListItem item in cblAllowedRoles.Items)
				{
					N2.Security.AuthorizedRole temp = new N2.Security.AuthorizedRole(this.SelectedItem, item.Value);
					int roleIndex = SelectedItem.AuthorizedRoles.IndexOf(temp);
					if (!item.Selected && roleIndex >= 0)
						SelectedItem.AuthorizedRoles.RemoveAt(roleIndex);
					else if (item.Selected && roleIndex < 0)
						SelectedItem.AuthorizedRoles.Add(temp);
				}
			}
			Engine.Persister.Save(SelectedItem);
		}

		private bool AllSelected(CheckBoxList cbl)
		{
			foreach (ListItem item in cbl.Items)
				if (!item.Selected)
					return false;
			return true;
		}

		private bool NoneSelected(CheckBoxList cbl)
		{
			foreach (ListItem item in cbl.Items)
				if (item.Selected)
					return false;
			return true;
		}

		protected void cblAllowedRoles_DataBound(object sender, EventArgs e)
		{
			if (SelectedItem.AuthorizedRoles.Count == 0)
			{
				foreach (ListItem item in cblAllowedRoles.Items)
					item.Selected = true;
			}
			else
			{
				foreach (N2.Security.AuthorizedRole allowedRole in SelectedItem.AuthorizedRoles)
				{
					cblAllowedRoles.Items.FindByValue(allowedRole.Role).Selected = true;
				}
			}
		}

		protected void cvSomethingSelected_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = !NoneSelected(this.cblAllowedRoles);
		}

		protected override void OnError(EventArgs e)
		{
			if(Server.GetLastError().GetType() == typeof(System.Reflection.TargetInvocationException))
			{
				string html = "<html><body><h1>This feature is not set up correctly or disabled</h1><p><i>Check &lt;configuration&gt; &lt;system.web&gt; &lt;membership&gt; ... in web.config</i></p><pre><h3>" + Server.GetLastError().Message + "</h3>" + Server.GetLastError().ToString() + "</pre></body></html>";
				Response.Write(html);
				Server.ClearError();
			}
			else
				base.OnError(e);
		}
	}
}
