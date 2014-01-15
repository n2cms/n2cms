using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using TextQuestion = N2.Templates.Items.TextQuestion;

namespace N2.Templates.Web.UI.WebControls
{
    public sealed class TextControl : Control, IQuestionControl
    {
        readonly TextBox _tb;
        readonly Label _l;
        readonly RequiredFieldValidator _rfv;

        public TextControl(TextQuestion question)
        {
            _tb = new TextBox { CssClass = "alternative" };
            if (question.ID > 0)
                _tb.ID = "q" + question.ID;

            if (question.Rows > 1)
            {
                _tb.TextMode = TextBoxMode.MultiLine;
                _tb.Rows = question.Rows;
            }

            if (question.Columns.HasValue)
            {
                _tb.Columns = question.Columns.Value;
            }

            _l = new Label { CssClass = "label", Text = question.Title };
            if (question.ID > 0)
                _l.AssociatedControlID = _tb.ID;

            Debug.Assert(Controls != null, "Controls != null");
            Controls.Add(_l);
            Controls.Add(_tb);

            if (!question.Required) return;

            _rfv = new RequiredFieldValidator
            {
                Display = ValidatorDisplay.Dynamic,
                Text = ControlPanel.RequiredField_DefaultText,
                ErrorMessage = string.Format(ControlPanel.RequiredField_DefaultErrorMessage, question.Title),
                ControlToValidate = _tb.ID,
                ValidationGroup = "Form"
            };
            Controls.Add(_rfv);
        }

        public string AnswerText
        {
            get { return _tb.Text; }
        }

        public string Question
        {
            get { return _l.Text; }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write(@"<div class=""row cf"">");
            base.Render(writer);
            writer.Write("</div>");
        }
    }
}
