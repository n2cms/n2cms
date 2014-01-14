using System;
using System.Linq;
using System.Web.Security;
using System.Web.UI.WebControls;
using N2.Edit.Web;

namespace N2.Edit.Membership
{
    public partial class EditRoles : EditPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var roleName = Request.QueryString["role"];

                var usersInRole = Roles.GetUsersInRole(roleName);

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

            Roles.RemoveUserFromRole(userName, roleName);

            Response.Redirect(Request.RawUrl);
        }
    }
}
