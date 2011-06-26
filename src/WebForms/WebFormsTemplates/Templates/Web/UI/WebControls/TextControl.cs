using System.Web.UI;
using System.Web.UI.WebControls;
using TextQuestion=N2.Templates.Items.TextQuestion;

namespace N2.Templates.Web.UI.WebControls
{
    public class TextControl : Control, IQuestionControl
    {
        TextBox tb;
        Label l;
		RequiredFieldValidator rfv;

        public TextControl(TextQuestion question)
        {
            tb = new TextBox();
            tb.CssClass = "alternative";
            tb.ID = "q" + question.ID;
            if (question.Rows > 1)
            {
                tb.TextMode = TextBoxMode.MultiLine;
                tb.Rows = question.Rows;
            }
            if (question.Columns.HasValue)
            {
                tb.Columns = question.Columns.Value;
            }

            l = new Label();
            l.CssClass = "label";
            l.Text = question.Title;
            l.AssociatedControlID = tb.ID;

            Controls.Add(l);
			Controls.Add(tb);

			if (question.Required)
			{
				rfv = new RequiredFieldValidator { Display = ValidatorDisplay.Dynamic, Text = "*" }; 
				rfv.ErrorMessage = question.Title + " is required";
				rfv.ControlToValidate = tb.ID;
				rfv.ValidationGroup = "Form";
				this.Controls.Add(rfv);
			}
        }

        public string AnswerText
        {
            get { return tb.Text; }
        }

        public string Question
        {
            get { return l.Text; }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write("<div class='row cf'>");
            base.Render(writer);
            writer.Write("</div>");
        }
    }
}