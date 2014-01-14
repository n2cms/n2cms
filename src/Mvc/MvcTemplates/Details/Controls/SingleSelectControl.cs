using System.Web.UI;
using System.Web.UI.WebControls;
using SingleSelect=N2.Templates.Mvc.Items.Items.SingleSelect;

namespace N2.Templates.Mvc.Details.Controls
{
    public class SingleSelectControl : Control, IQuestionControl
    {
        readonly RadioButtonList rbl;
        readonly Label l;

        public SingleSelectControl(SingleSelect question, RepeatDirection direction)
        {
            rbl = new RadioButtonList();
            rbl.CssClass = "alternatives";
            rbl.ID = "q" + question.ID;
            rbl.DataTextField = "Title";
            rbl.DataValueField = "ID";
            rbl.DataSource = question.GetChildren();
            rbl.RepeatLayout = RepeatLayout.Flow;
            rbl.RepeatDirection = direction;
            rbl.DataBind();

            l = new Label();
            l.CssClass = "label";
            l.Text = question.Title;
            l.AssociatedControlID = rbl.ID;

            Controls.Add(l);
            Controls.Add(rbl);
        }

        public int SelectedIndex
        {
            get { return rbl.SelectedIndex; }
        }

        public string SelectedValue
        {
            get { return rbl.SelectedValue; }
        }

        #region IQuestionControl Members

        public string AnswerText
        {
            get 
            {
                return rbl.SelectedIndex >= 0
                        ? rbl.SelectedItem.Text
                        : string.Empty;
            }
        }

        public string Question
        {
            get { return l.Text; }
        }

        #endregion

        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write("<div class='row cf'>");
            base.Render(writer);
            writer.Write("</div>");
        }
    }
}
