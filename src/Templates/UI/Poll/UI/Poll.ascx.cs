using System;
using System.Web.UI.WebControls;
using N2.Templates.Survey.Web.UI;

namespace N2.Templates.Poll.UI
{
	public partial class Poll : Templates.Web.UI.TemplateUserControl<Templates.Items.AbstractContentPage, Items.Poll>
	{
		protected PlaceHolder phQuestion;
		protected SingleSelectControl lcAlternatives;
		protected CustomValidator cvAlternative;

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			Resources.Register.StyleSheet(Page, "~/Poll/UI/Css/Poll.css", N2.Resources.Media.All);
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
				CurrentItem.AddAnswer(N2.Context.Persister, selectedItem);
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