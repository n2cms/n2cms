using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using N2.Web.UI;
using N2.Security;
using N2.Web;

namespace N2.Edit.Web.UI.Controls
{
	public class PermissionPanel : PlaceHolder
	{
		CustomValidator cv = new CustomValidator
		{
			CssClass = "info",
			Text = "You do not have sufficient permissions.",
			Display = ValidatorDisplay.Dynamic
		};

		public string Text
		{
			get { return cv.Text; }
			set { cv.Text = value; }
		}

		public Permission RequiredPermission { get; set; }

		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			cv.Page = Page;
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (RequiredPermission == Permission.None)
				RequiredPermission = Page.GetType().GetCustomAttributes(typeof(IPermittable), true).OfType<IPermittable>()
					.Select(p => PermissionMap.GetMaximumPermission(p.RequiredPermission))
					.OrderByDescending(rp => rp)
					.FirstOrDefault();

			var item = new SelectionUtility(this, Page.GetEngine()).SelectedItem;
			if (!Page.GetEngine().SecurityManager.IsAuthorized(Page.User, item, RequiredPermission))
			{
				var message = "User: " + Page.User.Identity.Name + "(" + GetUserRoles(Page.User.Identity.Name) + ")" + "  Item:" + item.GetType().Name + "_" + item.ID + "_" + item.Title + ",  RequiredPremission:" + RequiredPermission + ", AlteredPermissions:" + item.AlteredPermissions + " , Write_Roles: (" + GetRolesForPermission(item, Permission.Write) + ")" + " , Publish_Roles: (" + GetRolesForPermission(item, Permission.Publish) + ")"+" , Admin_Roles: (" + GetRolesForPermission(item, Permission.Administer) + ")";
				Page.GetEngine().Resolve<IErrorNotifier>().Notify(new UnauthorizedAccessException(message));
				cv.IsValid = false;
				cv.RenderControl(writer);
			}
			else
				base.Render(writer);
		}

		private string GetUserRoles(string userName)
		{
			AccountManager am = N2.Context.Current.Resolve<AccountManager>();
			string[] roles = am.GetAllRoles();
			var userRoles = new List<string>();
			foreach (string role in roles)
			{
				if (am.IsUserInRole(Page.User.Identity.Name, role))
				{
					userRoles.Add(role);
				}
			}
			return string.Join<string>(",", userRoles).ToString();
		}

		//private Permission GetItemRemapPermissions(ContentItem item, Permission permission)
		//{
		//	foreach (PermissionRemapAttribute remap in item.GetContentType().GetCustomAttributes(typeof(PermissionRemapAttribute), true))
		//		permission = remap.Remap(permission);

		//	return permission;
		//}

		private string GetRolesForPermission(ContentItem item, Permission permission) 
		{
			var roles = DynamicPermissionMap.GetRoles(item, permission);
			if (roles !=null && roles.Any())
				return string.Join(",",roles);
			else return "";
		}
	}
}
