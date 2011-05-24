using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Templates.Items;

namespace N2.Templates.Web.UI.WebControls
{
    public class MultipleSelectControl : Control, IQuestionControl
    {
        MultipleSelect item;
        CheckBoxList list;
        RequiredFieldValidatorForCheckBoxLists rfv;

        public MultipleSelectControl(MultipleSelect item, RepeatDirection direction)
        {
            this.item = item;
            list = new CheckBoxList();
            list.RepeatDirection = direction;
        }

        protected override void OnInit(EventArgs e)
        {
            Label l = new Label();
            l.Text = item.Title;
            l.CssClass = "label";
            l.AssociatedControlID = item.Name;
            
            list.ID = item.Name;
            list.CssClass = "alternatives";
            list.DataSource = item.GetChildren();
            list.DataTextField = "Title";
            list.DataValueField = "ID";
            list.DataBind();

            if (item["Required"] != null && (bool)item["Required"] == true)
            {
                rfv = new RequiredFieldValidatorForCheckBoxLists();
                rfv.ControlToValidate = list.ID;
                rfv.ErrorMessage = "Required Field";
                rfv.ValidationGroup = "Form";
                rfv.ID = "Required" + item.ID;
                rfv.SetFocusOnError = true;
                rfv.Enabled = true;
                rfv.Display = ValidatorDisplay.Dynamic;
                rfv.EnableClientScript = true;
                list.ValidationGroup = "Form";
                list.CausesValidation = true;

                Controls.Add(rfv);
            }

            this.Controls.Add(l);
            this.Controls.Add(list);
            base.OnInit(e);
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