using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Templates.Items;
using EditableOptionsAttribute=N2.Templates.Details.EditableOptionsAttribute;

namespace N2.Templates.Details
{
    public class PollCreatorDetailAttribute : EditableOptionsAttribute
    {
        private string createNewText = "Create new";
        private string questionText;

        public string CreateNewText
        {
            get { return createNewText; }
            set { createNewText = value; }
        }

        public string QuestionText
        {
            get { return questionText; }
            set { questionText = value; }
        }

        public override bool UpdateItem(ContentItem item, Control editor)
        {
            ContentItem questionItem = Utility.GetProperty(item, Name) as ContentItem;

            CheckBox cb = editor.FindControl(GetCheckBoxName()) as CheckBox;
            if (cb.Checked || questionItem == null)
            {
                questionItem = new SingleSelect();
                questionItem.AddTo(item);
                Utility.UpdateSortOrder(item.Children);
            }

            TextBox tb = editor.FindControl(GetTextBoxName()) as TextBox;
            questionItem.Title = tb.Text;
        
            return base.UpdateItem(questionItem, editor);
        }

        public override void UpdateEditor(ContentItem item, Control editor)
        {
            ContentItem questionItem = Utility.GetProperty(item, Name) as ContentItem 
                                       ?? new SingleSelect();

            TextBox tb = editor.FindControl(GetTextBoxName()) as TextBox;
            tb.Text = questionItem.Title;

            base.UpdateEditor(questionItem, editor);
        }

        public override Control AddTo(Control container)
        {
            Control panel = AddPanel(container);
            
            Label label = new Label();
            label.ID = Name + "-Label";
            label.Text = QuestionText;
            label.CssClass = "editorLabel";
            panel.Controls.Add(label);

            TextBox tb = new TextBox();
            tb.ID = GetTextBoxName();
            panel.Controls.Add(tb);

            CheckBox cb = new CheckBox();
            cb.ID = GetCheckBoxName();
            cb.Text = CreateNewText;
            panel.Controls.Add(cb);

            return base.AddTo(container);
        }

        private string GetTextBoxName()
        {
            return Name + "-Title";
        }

        private string GetCheckBoxName()
        {
            return Name + "-CreateNew";
        }
    }
}
