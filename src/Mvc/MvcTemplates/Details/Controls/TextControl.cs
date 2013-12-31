using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Templates.Mvc.Items.Items;

namespace N2.Templates.Mvc.Details.Controls
{
    public class TextControl : Control, IQuestionControl
    {
        TextBox tb;
        Label l;

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
