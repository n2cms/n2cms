using System;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Edit.Web;

namespace N2.Edit.Membership
{
    public partial class New : EditPage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            createUserWizard.Question = Guid.NewGuid().ToString();
            createUserWizard.Answer = Guid.NewGuid().ToString();
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
                    Roles.AddUserToRole(_cuw.UserName, item.Value);
        }

        protected void createUserWizard_FinishButtonClick(object sender, EventArgs e)
        {
            Response.Redirect("Users.aspx");
        }
    }
}
