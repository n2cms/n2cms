using System;
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
        public const int PageSize = 100;

        public static MembershipUserCollection GetUsers(int start, int max)
        {
            int page = start/max;
            int total;
            MembershipUserCollection users = System.Web.Security.Membership.GetAllUsers(page, max, out total);

            return users;
        }

        public static int GetUsersCount()
        {
            int total;
            System.Web.Security.Membership.GetAllUsers(0, PageSize, out total);

            return total;
        }

        public static void DeleteUser(string userName)
        {
            if (userName == null)
                throw new ArgumentNullException("userName");

            System.Web.Security.Membership.DeleteUser(userName, true);
        }
    }
}
