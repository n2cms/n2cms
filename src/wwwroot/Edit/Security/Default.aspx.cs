using System;
using System.Web.Security;
using System.Web.UI.WebControls;
using N2.Security;
using System.Collections.Generic;

namespace N2.Edit.Security
{
	[N2.Edit.ToolbarPlugin("", "security", "~/Edit/Security/Default.aspx?selected={selected}", ToolbarArea.Preview, Targets.Preview, "~/Edit/Img/Ico/lock.gif", 100, ToolTip = "allowed roles for selected item", GlobalResourceClassName = "Toolbar")]
	public partial class Default : Web.EditPage
	{
		protected override void OnInit(EventArgs e)
		{
            string[] roles;
            if (Roles.Enabled)
                roles = Roles.GetAllRoles();
            else
                roles = GetRoles();
            cblAllowedRoles.DataSource = roles;
			cblAllowedRoles.DataBind();
			base.OnInit(e);
		}

        private string[] GetRoles()
        {
            List<string> roles = new List<string>();
            roles.Add(AuthorizedRole.Everyone);
            if (Engine.SecurityManager is N2.Security.SecurityManager)
            {
                SecurityManager sm = Engine.SecurityManager as SecurityManager;
                roles.AddRange(sm.EditorRoles);
                roles.AddRange(sm.AdminRoles);
            }
            else
            {
                roles.Add("Administrators");
            }

            return roles.ToArray();
        }

        private object SecurityManager(SecurityManager securityManager)
        {
            throw new NotImplementedException();
        }

        

		protected void Page_Load(object sender, EventArgs e)
		{
            // Set text for everyone
            cbEveryone.Text = AuthorizedRole.Everyone;

            hlCancel.NavigateUrl = CancelUrl();
		}

		protected void cblAllowedRoles_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		protected void btnSave_Command(object sender, CommandEventArgs e)
		{
            ApplyRoles(SelectedItem);

			base.Refresh(SelectedItem, ToolbarArea.Navigation);
		}

        protected void btnSaveRecursive_Command(object sender, CommandEventArgs e)
		{
            ApplyRolesRecursive(SelectedItem);

			base.Refresh(SelectedItem, ToolbarArea.Navigation);
		}

        private void ApplyRolesRecursive(ContentItem item)
        {
            ApplyRoles(item);
            foreach (ContentItem child in item.GetChildren())
            {
                ApplyRolesRecursive(child);
            }
        }

        private void ApplyRoles(ContentItem item)
        {
            // Check if everyone is checked
            if (cbEveryone.Checked)
            {
                // Clear the current roles
                item.AuthorizedRoles.Clear();
            }
            else
            {
                foreach (ListItem li in cblAllowedRoles.Items)
                {
                    AuthorizedRole temp = new AuthorizedRole(item, li.Value);
                    int roleIndex = item.AuthorizedRoles.IndexOf(temp);
                    if (!li.Selected && roleIndex >= 0)
                        item.AuthorizedRoles.RemoveAt(roleIndex);
                    else if (li.Selected && roleIndex < 0)
                        item.AuthorizedRoles.Add(temp);
                }
            }
            Engine.Persister.Save(item);
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
            if (cbEveryone.Checked)
                return false;
			foreach (ListItem item in cbl.Items)
				if (item.Selected)
					return false;
			return true;
		}

		protected void cblAllowedRoles_DataBound(object sender, EventArgs e)
		{
			if (SelectedItem.AuthorizedRoles.Count == 0)
			{
                // Everyone is allowed
                cbEveryone.Checked = true;
                cblAllowedRoles.Enabled = false;

				/*foreach (ListItem item in cblAllowedRoles.Items)
					item.Selected = true;*/
			}
			else
			{
                // Uncheck everyone
                cbEveryone.Checked = false;
                cblAllowedRoles.Enabled = true;
                
                // Check all allowed roles
				foreach (N2.Security.AuthorizedRole allowedRole in SelectedItem.AuthorizedRoles)
				{
					cblAllowedRoles.Items.FindByValue(allowedRole.Role).Selected = true;
				}
			}
		}

        protected void cbEveryone_CheckedChanged(object sender, EventArgs e)
        {
            // Check if the user checked everyone
            cblAllowedRoles.Enabled = !cbEveryone.Checked;
        }

		protected void cvSomethingSelected_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = !NoneSelected(this.cblAllowedRoles);
		}

		protected override void OnError(EventArgs e)
		{
			if(Server.GetLastError().GetType() == typeof(System.Reflection.TargetInvocationException))
			{
                string html = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html>
    <head>
	    <link rel=""stylesheet"" href=""../Css/All.css"" type=""text/css"" />
        <link rel=""stylesheet"" href=""../Css/Framed.css"" type=""text/css"" />
    </head>
    <body><div class='content'>
        <h1>This feature might not have been enabled in web.config. Please look into ASP.NET roles configuration.</h1>
        <p><i>Check &lt;configuration&gt; &lt;system.web&gt; &lt;roleManager&gt; ... in web.config</i></p><pre><h3>"
                    + Server.GetLastError().Message 
                    + "</h3>" 
                    + Server.GetLastError().ToString() + "</pre></div></body></html>";

                Response.Write(html);
				Server.ClearError();
			}
			else
				base.OnError(e);
		}
	}
}
