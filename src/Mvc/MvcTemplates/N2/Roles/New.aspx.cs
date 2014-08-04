using System;
using System.Linq;
using System.Web.Security;
using System.Web.UI.WebControls;
using N2.Edit.Web;
using N2.Security;

namespace N2.Edit.Membership
{
    public partial class NewRole : EditPage
    {
        private AccountManager AccountManager { get { return N2.Context.Current.Resolve<AccountManager>(); } }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnCreateRoleClick(object sender, EventArgs e)
        {
            if (IsValid)
            {
                var roleName = txtRoleName.Text;
                AccountManager.CreateRole(roleName);
                // REMOVE: Roles.CreateRole(roleName);

                Response.Redirect("Roles.aspx");
            }
        }

        protected void ValidateNewGroup(object source, ServerValidateEventArgs args)
        {
            args.IsValid = AccountManager.IsValidRole(args.Value);
            // REMOVE: args.IsValid = !Roles.GetAllRoles().Any(r => r.Equals(args.Value, StringComparison.OrdinalIgnoreCase));
        }
    }
}
