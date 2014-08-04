using System;
using System.Linq;
using System.Web.Security;
using System.Web.UI.WebControls;
using N2.Edit.Web;
using N2.Security;

namespace N2.Edit.Membership
{
    [MembershipToolbarPlugin("ROLES", "roles", "~/N2Extensions/Roles/Roles.aspx", "{ManagementUrl}/Resources/icons/group.png", 111,
        ToolTip = "administer roles",
        GlobalResourceClassName = "Toolbar",
        RequiredPermission = Permission.Administer)]
    public partial class ListRoles : EditPage
    {
        private AccountManager AccountManager { get { return N2.Context.Current.Resolve<AccountManager>(); } }
        // REMOVE: private static readonly string[] SystemRoles = new [] {"Administrators", "Editors", "Writers"};

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dgrRoles.DataSource = AccountManager.GetAllRoles().Select(x => new { RoleName = x });
                // REMOVE: dgrRoles.DataSource = Roles.GetAllRoles().Select(x => new { RoleName = x });
                dgrRoles.DataBind();
            }
        }

        protected void dgrRoles_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var roleName = (string) dgrRoles.DataKeys[e.Row.RowIndex].Value;

                if (AccountManager.IsSystemRole(roleName) ||
                    AccountManager.HasUsersInRole(roleName))
                /* REMOVE: if (SystemRoles.Contains(roleName, StringComparer.InvariantCultureIgnoreCase) ||
                    Roles.GetUsersInRole(roleName).Any())
                 */
                {
                    e.Row.Cells[1].Controls.Clear();
                }
            }
        }

        protected void dgrRoles_OnRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var roleName = (string) e.Keys[0];
            AccountManager.DeleteRole(roleName);
            // REMOVE: Roles.DeleteRole(roleName);

            Response.Redirect(Request.RawUrl);
        }
    }
}
