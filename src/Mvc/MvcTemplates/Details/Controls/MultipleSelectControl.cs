using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Templates.Mvc.Items.Items;

namespace N2.Templates.Mvc.Details.Controls
{
    public class MultipleSelectControl : Control, IQuestionControl
    {
        MultipleSelect item;
        CheckBoxList list;

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
            this.Controls.Add(l);

            list.ID = item.Name;
            list.CssClass = "alternatives";
            list.DataSource = item.GetChildren();
            list.DataTextField = "Title";
            list.DataValueField = "ID";
            list.DataBind();
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
