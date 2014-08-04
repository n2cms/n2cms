using System;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Edit.Web;
using N2.Security;

namespace N2.Edit.Membership
{
    public partial class New : EditPage
    {
        // Note: CreateUserWizard is used internally what may not be appropriate for all account subsystems.
        //       See AccountResources to register a custom add-new-user page. 
        private AccountManager AccountManager { get { return N2.Context.Current.Resolve<AccountManager>(); } }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            createUserWizard.Question = Guid.NewGuid().ToString();
            createUserWizard.Answer = Guid.NewGuid().ToString();
            createUserWizard.CreatingUser += createUserWizard_CreatingUser;

			((Button)createUserWizard.CreateUserStep.CustomNavigationTemplateContainer.FindControl("StepNextButtonButton")).CssClass += " btn";
        }

        protected bool IsMembershipAccountType()
        {
            return AccountManager.IsMembershipAccountType();
        }

        protected void OnCreateClick(object sender, EventArgs e)
		{
		}

        private void createUserWizard_CreatingUser(object sender, LoginCancelEventArgs e)
        {
           if (!IsMembershipAccountType())
           {
              // The page is not supported for this type of membership. 
              // Configure account subsystem to use a specialized page to serve the request.
              e.Cancel = true;
           }
        }

        protected void createUserWizard_CreatedUser(object sender, EventArgs e)
        {
            var _cuw = sender as CreateUserWizard;
            var cblRoles = _cuw.CreateUserStep.ContentTemplateContainer.FindControl("cblRoles") as CheckBoxList;

            //Bug in CreateUserWizard:
            // UserName property is only being taken from a ViewState,
            // on a blind assumption that it's already there.
            // Though, it might well be untrue if
            // TextBox.OnTextChanged post-back event (which populates UserName)
            // will be fired later than Button.OnClick (which tries to consume it)
            if (string.IsNullOrEmpty(_cuw.UserName)) {
                _cuw.UserName = (_cuw.CreateUserStep.ContentTemplateContainer.FindControl("UserName") as ITextControl).Text;
            }

            foreach (ListItem item in cblRoles.Items)
                if (item.Selected)
                    AccountManager.AddUserToRole(_cuw.UserName, item.Value);
                    // REMOVE: Roles.AddUserToRole(_cuw.UserName, item.Value);
        }

        protected void createUserWizard_FinishButtonClick(object sender, EventArgs e)
        {
            Response.Redirect("Users.aspx");
        }
    }
}
