using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Configuration;
using N2.Security;

namespace N2.Edit.Security
{
	[ToolbarPlugin(
		/* title */ "PERM",
		/* name */ "security",
		/* url format */ "{ManagementUrl}/Content/Security/Default.aspx?{Selection.SelectedQueryKey}={selected}",
		/* toolbar */ ToolbarArea.Preview,
		/* target */ Targets.Preview,
		/* icon */ "{ManagementUrl}/Resources/icons/key.png",
		/* sort */ 100,
		ToolTip = "allowed roles for selected item",
		GlobalResourceClassName = "Toolbar",
		RequiredPermission = Permission.Publish,
		Legacy = true)]
	public partial class Default : Web.EditPage
	{
		private AccountManager AccountManager { get { return N2.Context.Current.Resolve<AccountManager>(); } }
		protected Permission[] Permissions;
		protected string[] Roles;
		Control[,] map;

		protected override void OnInit(EventArgs e)
		{
			InitValues();
			base.OnInit(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			if (!IsPostBack)
				DataBind();
			try
			{
				Page.Title += @": " + Selection.SelectedItem.Title;
			}
			catch
			{
			}
			base.OnLoad(e);
		}

		protected void rptPermissions_ItemCreated(object sender, RepeaterItemEventArgs args)
		{
			if (args.Item.ItemType != ListItemType.Item && args.Item.ItemType != ListItemType.AlternatingItem) return;
			int roleIndex = ((RepeaterItem)((Control)sender).Parent).ItemIndex;
			map[args.Item.ItemIndex, roleIndex] = args.Item;
		}



		protected void btnSave_Command(object sender, CommandEventArgs e)
		{
			Validate();
			if (!IsValid)
				return;

			ApplyRoles(Selection.SelectedItem);
			InitValues();
			DataBind();

			base.Refresh(Selection.SelectedItem, ToolbarArea.Both);
		}

		protected void btnSaveRecursive_Command(object sender, CommandEventArgs e)
		{
			Validate();
			if (!IsValid)
				return;

			ApplyRolesRecursive(Selection.SelectedItem);
			DataBind();

			base.Refresh(Selection.SelectedItem, ToolbarArea.Both);
		}



		void InitValues()
		{
			Permissions = GetAvailablePermissions();
			Roles = GetAvailableRoles();
			map = new Control[Permissions.Length, Roles.Length];
		}

		Permission[] GetAvailablePermissions()
		{
			var config = Engine.Resolve<EditSection>();
			var permissions = new List<Permission> { Permission.Read };
			if (config.Editors.Dynamic)
				permissions.Add(Permission.Write);
			if (config.Writers.Dynamic)
				permissions.Add(Permission.Publish);
			if (config.Administrators.Dynamic)
				permissions.Add(Permission.Administer);
			return permissions.ToArray();
		}

		protected bool IsEveryone(Permission permission)
		{
			return DynamicPermissionMap.IsAllRoles(Selection.SelectedItem, permission);
		}

		protected bool IsRolePermitted(string role, Permission permission)
		{
			return IsRolePermitted(Selection.SelectedItem, role, permission);
		}

		protected bool IsRolePermitted(ContentItem item, string role, Permission permission)
		{
			GenericPrincipal tempUser = TempUserWithRole(role);
			bool isAuthorized = Engine.SecurityManager.IsAuthorized(tempUser, item, permission);
			return isAuthorized;
		}

		protected bool IsUserPermitted(string role, Permission permission)
		{
			if (Engine.SecurityManager.IsAdmin(User))
				return true;

			bool isInRole = User.IsInRole(role);
			bool isAuthorized = Engine.SecurityManager.IsAuthorized(User, Selection.SelectedItem, permission);
			return isInRole && isAuthorized;
		}

		protected string GetRole(RepeaterItem repeaterItem)
		{
			return (string)((RepeaterItem)repeaterItem.Parent.Parent).DataItem;
		}

		protected string[] GetAvailableRoles()
		{
			if (AccountManager.AreRolesEnabled())
                 return AccountManager.GetAllRoles();
            /* REMOVE: if (System.Web.Security.Roles.Enabled)
				return System.Web.Security.Roles.GetAllRoles();
            */
			
			var roles = new List<string> { AuthorizedRole.Everyone };
			if (Engine.SecurityManager is SecurityManager)
			{
				var sm = Engine.SecurityManager as SecurityManager;
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
			foreach (ContentItem child in item.Children.WhereAccessible())
			{
				ApplyRolesRecursive(child);
			}
		}

		private void ApplyRoles(ContentItem item)
		{
			for (int i = 0; i < Permissions.Length; i++)
			{
				Permission permission = Permissions[i];

				if (!IsAuthorized(Selection.SelectedItem, permission))
					continue;

				if (IsDefaultChecked(i))
				{
					DynamicPermissionMap.SetAllRoles(item, permission);
				}
				else
				{
					List<string> checkedRoles = new List<string>();
					for (int j = 0; j < Roles.Length; j++)
					{
						string role = Roles[j];
						CheckBox cb = (CheckBox)map[i, j].FindControl("cbRole");

						// try to avoid security breaches
						if (!IsUserPermitted(role, permission))
							cb.Checked = IsRolePermitted(role, permission);

						if (cb.Checked)
							checkedRoles.Add(role);
					}
					DynamicPermissionMap.SetRoles(item, permission, checkedRoles.ToArray());
				}
			}

			Engine.Persister.Save(item);
		}

		protected void cvSomethingSelected_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = true;
			for (int i = 0; i < Permissions.Length; i++)
			{
				if (IsDefaultChecked(i))
					continue;

				bool anyoneChecked = false;
				for (int j = 0; j < Roles.Length; j++)
				{
					if (!((CheckBox)map[i, j].FindControl("cbRole")).Checked)
						continue; // not checked, move on to the next one

					// checked- we're done with this row
					anyoneChecked = true;
					break;
				}

				if (anyoneChecked)
					continue; // at least this row has 1 checkbox selected, we're OK

				var validator = ((CustomValidator)rptEveryone.Items[i].FindControl("cvMarker"));
				validator.IsValid = args.IsValid = false;
			}
		}

		bool IsDefaultChecked(int permissionIndex)
		{
			return ((CheckBox)rptEveryone.Items[permissionIndex].FindControl("cbEveryone")).Checked;
		}

		GenericPrincipal TempUserWithRole(string role)
		{
			return new GenericPrincipal(new GenericIdentity("TempUser"), new[] { role });
		}
	}
}
