using System;
using System.Web.UI.WebControls;
using N2.Configuration;
using N2.Security;
using System.Collections.Generic;
using System.Web.UI;

namespace N2.Edit.Security
{
	[ToolbarPlugin("", "security", "~/Edit/Security/Default.aspx?selected={selected}", ToolbarArea.Preview, Targets.Preview, "~/Edit/Img/Ico/lock.gif", 100, 
		AuthorizedRoles = new[]{ "Editors", "Administrators", "Admin" },
		ToolTip = "allowed roles for selected item", 
		GlobalResourceClassName = "Toolbar")]
	public partial class Default : Web.EditPage
	{
		protected Permission[] Permissions;
		protected string[] Roles;
		Control[,] map;

		protected override void OnInit(EventArgs e)
		{
			InitValues();

			hlCancel.NavigateUrl = CancelUrl();
			base.OnInit(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			if(!IsPostBack)
			{
				DataBind();
			}

			base.OnLoad(e);
		}



		protected void rptPermissions_ItemDataBound(object sender, RepeaterItemEventArgs args)
		{
			CheckBox cb = (CheckBox)args.Item.FindControl("cbRole");
		}

		protected void rptPermissions_ItemCreated(object sender, RepeaterItemEventArgs args)
		{
			if(args.Item.ItemType == ListItemType.Item || args.Item.ItemType == ListItemType.AlternatingItem)
			{
				int roleIndex = ((RepeaterItem) ((Control) sender).Parent).ItemIndex;
				map[args.Item.ItemIndex, roleIndex] = args.Item;
			}
		}



		protected void btnSave_Command(object sender, CommandEventArgs e)
		{
			Validate();
			if(!IsValid)
				return;

			ApplyRoles(SelectedItem);
			InitValues();
			DataBind();

			base.Refresh(SelectedItem, ToolbarArea.Navigation);
		}

		protected void btnSaveRecursive_Command(object sender, CommandEventArgs e)
		{
			Validate();
			if (!IsValid)
				return;

			ApplyRolesRecursive(SelectedItem);
			DataBind();

			base.Refresh(SelectedItem, ToolbarArea.Navigation);
		}



		void InitValues()
		{
			Permissions = GetAvailablePermissions();
			Roles = GetAvailableRoles();
			map = new Control[Permissions.Length, Roles.Length];
		}
		
		Permission[] GetAvailablePermissions()
		{
			List<Permission> permissions = new List<Permission>();
			permissions.Add(Permission.Read);
			EditSection config = Engine.Resolve<EditSection>();
			if (config.Editors.Dynamic)
				permissions.Add(Permission.Write);
			if (config.Writers.Dynamic)
				permissions.Add(Permission.Publish);
			if(config.Administrators.Dynamic)
				permissions.Add(Permission.Administer);
			return permissions.ToArray();
		}

		protected bool IsEveryone(Permission permission)
		{
			return DynamicPermissionMap.IsAllRoles(SelectedItem, permission);
		}

		protected bool Is(string role, Permission permission)
		{
			return DynamicPermissionMap.IsPermitted(role, SelectedItem, permission);
		}

		protected bool IsAuthorized(string role, Permission permission)
		{
			return User.IsInRole(role) && Engine.SecurityManager.IsAuthorized(User, SelectedItem, permission);
		}

		protected string GetRole(RepeaterItem repeaterItem)
		{
			return (string)((RepeaterItem)repeaterItem.Parent.Parent).DataItem;
		}

        protected string[] GetAvailableRoles()
        {
			if (System.Web.Security.Roles.Enabled)
				return System.Web.Security.Roles.GetAllRoles();

            List<string> roles = new List<string>();
            roles.Add(AuthorizedRole.Everyone);
            if (Engine.SecurityManager is SecurityManager)
            {
                SecurityManager sm = Engine.SecurityManager as SecurityManager;
                roles.AddRange(sm.Writers.Roles);
                roles.AddRange(sm.Editors.Roles);
				roles.AddRange(sm.Administrators.Roles);
            }
            else
            {
                roles.Add("Administrators");
            }

            return roles.ToArray();
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
        	foreach(CheckBox c in N2.Web.UI.ItemUtility.FindInChildren<CheckBox>(rptPermittedRoles))
        	{
        		Console.WriteLine(c.Checked);
        	}

        	for (int i = 0; i < Permissions.Length; i++)
        	{
        		Permission permission = Permissions[i];
				bool allChecked = IsAllChecked(i);
				List<string> checkedRoles = new List<string>();
				if(!allChecked)
				{
					allChecked = true;
					for (int j = 0; j < Roles.Length; j++)
					{
						string role = Roles[j];
						CheckBox cb = (CheckBox)map[i, j].FindControl("cbRole");

						// try to avoid security breaches
						if (!IsAuthorized(role, permission))
							cb.Checked = Is(role, permission);
						
						allChecked &= cb.Checked;
						if (cb.Checked)
						{
							checkedRoles.Add(role);
						}
					}
				}

				if(allChecked)
					DynamicPermissionMap.SetAllRoles(item, permission);
				else
					DynamicPermissionMap.SetRoles(item, permission, checkedRoles.ToArray());
			}

			Engine.Persister.Save(item);
        }

		protected void cvSomethingSelected_ServerValidate(object source, ServerValidateEventArgs args)
		{
			for (int i = 0; i < Permissions.Length; i++)
			{
				if (IsAllChecked(i))
					continue;

				bool anyoneChecked = false;
				for (int j = 0; j < Roles.Length; j++)
				{
					string role = Roles[j];
					CheckBox cb = (CheckBox) map[i, j].FindControl("cbRole");
					anyoneChecked |= cb.Checked;
				}

				if(!anyoneChecked)
				{
					CustomValidator validator = ((CustomValidator)rptEveryone.Items[i].FindControl("cvMarker"));
					validator.IsValid = false;

					args.IsValid = false;
				}
			}
		}

		bool IsAllChecked(int permissionIndex)
		{
			return ((CheckBox)rptEveryone.Items[permissionIndex].FindControl("cbEveryone")).Checked;
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
