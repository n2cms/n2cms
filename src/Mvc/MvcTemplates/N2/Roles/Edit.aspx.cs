using System;
using System.Linq;
using System.Web.Security;
using System.Web.UI.WebControls;
using N2.Edit.Web;
using N2.Security;

namespace N2.Edit.Membership
{
    public partial class EditRoles : EditPage
    {
        private AccountManager AccountManager { get { return N2.Context.Current.Resolve<AccountManager>(); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var roleName = Request.QueryString["role"];

                var usersInRole = AccountManager.GetUsersInRole(roleName);
                // REMOVE: var usersInRole = Roles.GetUsersInRole(roleName);

                if (usersInRole.Any())
                {
                    dgrUsers.DataSource = usersInRole.Select(x => new {UserName = x});
                    dgrUsers.DataBind();
                }
                else
                {
                    dgrUsers.Visible = false;
                    htmNoUsers.Visible = true;
                }

                Title = roleName;
            }
        }

        protected void dgrUsers_OnRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var roleName = Request.QueryString["role"];
            var userName = (string) e.Keys[0];

            AccountManager.RemoveUserFromRole(userName, roleName);
            // REMOVE: Roles.RemoveUserFromRole(userName, roleName);

            Response.Redirect(Request.RawUrl);
        }
    }
}
