using System.Web.UI;
using System.Web.UI.WebControls;
using SingleSelect = N2.Templates.Items.SingleSelect;

namespace N2.Templates.Web.UI.WebControls
{
    public class SingleSelectControl : Control, IQuestionControl
    {
        readonly ListControl lc;
        readonly Label l;
        readonly CustomValidator cv;

        public SingleSelectControl(SingleSelect question)
        {
            RepeatDirection direction = question.Vertical ? RepeatDirection.Vertical : RepeatDirection.Horizontal;

            switch (question.SelectionType)
            {
                case Items.SingleSelectType.DropDown:
                    lc = new DropDownList();
                    break;
                case Items.SingleSelectType.ListBox:
                    var lb = new ListBox();
                    lb.SelectionMode = ListSelectionMode.Single;
                    lc = lb;
                    break;
                case Items.SingleSelectType.RadioButtons:
                    var rbl = new RadioButtonList();
                    rbl.RepeatLayout = RepeatLayout.Flow;
                    rbl.RepeatDirection = direction;
                    lc = rbl;
                    break;
            }

            lc.CssClass = "alternatives";
            if (question.ID > 0)
                lc.ID = "q" + question.ID;
            lc.DataTextField = "Title";
            lc.DataValueField = "ID";
			lc.DataSource = question.Children.WhereAccessible();
            lc.DataBind();

            l = new Label();
            l.CssClass = "label";
            l.Text = question.Title;
            if (question.ID > 0)
                l.AssociatedControlID = lc.ID;

            Controls.Add(l);
            Controls.Add(lc);

            if (question.Required)
            {
                cv = new CustomValidator { Display = ValidatorDisplay.Dynamic, Text = "*" };
                cv.ErrorMessage = question.Title + " is required";
                cv.ServerValidate += (s, a) => a.IsValid = !string.IsNullOrEmpty(AnswerText);
                cv.ValidationGroup = "Form";
                Controls.Add(cv);
            }
        }

        public int SelectedIndex
        {
            get { return lc.SelectedIndex; }
        }

        public string SelectedValue
        {
            get { return lc.SelectedValue; }
        }

        #region IQuestionControl Members

        public string AnswerText
        {
            get
            {
                return lc.SelectedIndex >= 0
                           ? lc.SelectedItem.Text
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
