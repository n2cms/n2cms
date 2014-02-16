using System;
using System.Collections.Generic;
using System.Web.Security;
using System.Web.UI.WebControls;
using N2.Edit.Web;
using N2.Security;

namespace N2.Edit.Membership
{
	[MembershipToolbarPlugin("USERS", "users", "{ManagementUrl}/Users/Users.aspx", "{ManagementUrl}/Resources/icons/group_key.png", 110,
		ToolTip = "administer users",
		GlobalResourceClassName = "Toolbar",
		RequiredPermission = Permission.Administer)]
	public partial class Users : EditPage
	{
        
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			dgrUsers.PageSize = UsersSource.PageSize;
		}

		protected void dgrUsers_OnRowDeleting(object sender, GridViewDeleteEventArgs e)
		{
			odsUsers.DeleteParameters.Add("userName", (string)e.Keys[0]);
			odsUsers.Delete();
		}
	}


	public static class UsersSource
	{
        private static AccountManager AccountManager { get { return N2.Context.Current.Resolve<AccountManager>(); } }

        public static int PageSize { get { return AccountManager.PageSize; } }

        public static IList<IAccountInfo> GetUsers(int start, int max)
		{
            return AccountManager.GetUsers(start,max);
		}

		public static int GetUsersCount()
		{
	        return AccountManager.GetUsersCount();
		}

		public static void DeleteUser(string userName)
		{
            AccountManager.DeleteUser(userName);
		}
	}
}
