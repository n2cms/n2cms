using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Templates.Items;

namespace N2.Templates.Web.UI.WebControls
{
    public class MultipleSelectControl : Control, IQuestionControl
    {
        Label l;
        MultipleSelect item;
        CheckBoxList list;
        CustomValidator cv;

        public MultipleSelectControl(MultipleSelect item, RepeatDirection direction)
        {
            this.item = item;

            l = new Label();
            l.Text = item.Title;
            l.CssClass = "label";
            if (item.ID > 0)
                l.AssociatedControlID = "q" + item.ID;
            this.Controls.Add(l);

            list = new CheckBoxList();
            list.RepeatDirection = direction;
            if (item.ID > 0)
                list.ID = "q" + item.ID;
            list.CssClass = "alternatives";
			list.DataSource = item.Children.WhereAccessible();
            list.DataTextField = "Title";
            list.DataValueField = "ID";
            list.DataBind();
            this.Controls.Add(list);

            if (item.Required)
            {
                cv = new CustomValidator { Display = ValidatorDisplay.Dynamic, Text = "*" };
                cv.ErrorMessage = item.Title + " is required";
                cv.ServerValidate += (s, a) => a.IsValid = !string.IsNullOrEmpty(AnswerText);
                cv.ValidationGroup = "Form";
                this.Controls.Add(cv);
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write("<div class='row cf'>");
            base.Render(writer);
            writer.Write("</div>");
        }

        public string AnswerText
        {
            get
            {
                string answer = string.Empty;
                foreach (ListItem li in list.Items)
                {
                    if (li.Selected)
                    {
                        answer += li.Text + Environment.NewLine;
                    }
                }
                return answer;
            }
        }

        public string Question
        {
            get { return item.Title; }
        }
    }
}
