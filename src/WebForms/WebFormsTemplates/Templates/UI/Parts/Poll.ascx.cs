using System;
using System.Web.UI.WebControls;
using N2.Templates.Web.UI.WebControls;

namespace N2.Templates.UI.Parts
{
    public partial class Poll : Templates.Web.UI.TemplateUserControl<ContentItem, N2.Templates.Items.Poll>
    {
        protected PlaceHolder phQuestion;
        protected SingleSelectControl lcAlternatives;
        protected CustomValidator cvAlternative;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Resources.Register.StyleSheet(Page, "~/Templates/UI/Css/Poll.css", N2.Resources.Media.All);
            if (CurrentItem.Question != null)
            {
                lcAlternatives = CurrentItem.Question.AddTo(phQuestion) as SingleSelectControl;
                cvAlternative.ControlToValidate = lcAlternatives.ID;
                cvAlternative.Enabled = true;
            }
        }

        protected void cbAlternative_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = lcAlternatives.SelectedIndex >= 0;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Page.Validate("Poll");
            if (Page.IsValid)
            {
                int selectedItem = int.Parse(lcAlternatives.SelectedValue);
                CurrentItem.AddAnswer(Engine.Persister, selectedItem);
                Response.Cookies.Add(CurrentItem.GetAnsweredCookie(selectedItem));
                Response.Redirect(CurrentPage.Url);
            }
        }

        protected string GetShowUrl()
        {
            return CurrentPage.Url + Utility.Evaluate(CurrentItem, "Question.ID", "?p{0}=show");
        }
    }
}
