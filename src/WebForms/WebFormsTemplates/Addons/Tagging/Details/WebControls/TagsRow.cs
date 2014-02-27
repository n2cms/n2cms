using System.Web.UI.WebControls;
using System.Collections.Generic;
using N2.Addons.Tagging.Details.WebControls;
using System.Web.UI;

namespace N2.Addons.Tagging.Details.WebControls
{
    internal class TagsRow : TableRow, INamingContainer
    {
        readonly TagsEditor table;
        ListBox AvailableList { get; set; }
        TextBox AdditionalBox { get; set; }
        ListBox AddedList { get; set; }
        Button UntagButton { get; set; }
        Button TagButton { get; set; }
        Button AddButton { get; set; }
        Label Label { get; set; }
        RequiredFieldValidator AddValidator { get; set; }

        public TagsRow(TagsEditor table)
        {
            this.table = table;

            AddedList = new ListBox();
            TagButton = new Button();
            UntagButton = new Button();
            
            AddButton = new Button();
            AdditionalBox = new TextBox();
            AddValidator = new RequiredFieldValidator();
            AvailableList = new ListBox();
            Label = new Label();
        }

        private void AddNewTag(string tagName)
        {
            if (AddedList.Items.FindByValue(tagName) != null )
                return;

            if (AvailableList.Items.FindByValue(tagName) != null)
            {
                var li = AvailableList.Items.FindByValue(tagName);
                AvailableList.Items.Remove(li);
                AddedList.Items.Add(li);
                return;
            }
            
            AddedList.Items.Add(tagName);
        }



        private void MoveBetween(ListBox from, ListBox to)
        {
            while(from.SelectedItem != null)
            {
                ListItem selected = from.SelectedItem;

                from.Items.Remove(selected);
                to.Items.Add(selected);
            }
        }

        public void BindTo(IGroup container)
        {
            EnsureChildControls();

            Label.Text = container.Title;

            if(AvailableList.Items.Count != 0 || AddedList.Items.Count > 0)
                return;
            
            foreach (ITag tag in container.GetTags())
            {
                ListItem item = new ListItem(tag.Title + " (" + tag.ReferenceCount + ")", tag.Title);
                AvailableList.Items.Add(item);
            }
            
            string validationGroup = "AddTo" + container.Name;
            AddButton.ValidationGroup = validationGroup;
            AdditionalBox.ValidationGroup = validationGroup;
            AddValidator.ValidationGroup = validationGroup;         
        }

        public void Select(IEnumerable<string> selectedTags)
        {
            foreach(string tagName in selectedTags)
            {
                ListItem li = AvailableList.Items.FindByValue(tagName);
                if(li != null)
                {
                    AvailableList.Items.Remove(li);
                    AddedList.Items.Add(li);
                }
            }
        }

    
        
        protected override void OnInit(System.EventArgs e)
        {
            CssClass = "tag";

            Label.CssClass = "editorLabel";
            AddedList.ID = "lbAdded";
            AddedList.SelectionMode = ListSelectionMode.Multiple;
            TagButton.Text = "<";
            TagButton.CausesValidation = false;
            TagButton.ID = "btnTag";
            UntagButton.Text = ">";
            UntagButton.CausesValidation = false;
            UntagButton.ID = "btnUntag";
            AdditionalBox.ID = "txtAdditional";
            AddButton.Text = "+";
            AvailableList.ID = "lbAvailable"; 
            AvailableList.SelectionMode = ListSelectionMode.Multiple;

            EnsureChildControls();

            TagButton.Click += delegate
            {
                table.HasChanges = true;
                MoveBetween(AvailableList, AddedList);
            };
            UntagButton.Click += delegate
            {
                table.HasChanges = true;
                MoveBetween(AddedList, AvailableList);
            };
            AddButton.Click += delegate
            {
                table.HasChanges = true;
                AddNewTag(AdditionalBox.Text);
                AdditionalBox.Text = "";
            };

            base.OnInit(e);
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            TableCell labelCell = new TableCell();
            labelCell.CssClass = "tagsLabel";
            TableCell leftCell = new TableCell();
            leftCell.CssClass = "tagsLeft";
            Panel addPanel = new Panel();
            TableCell middleCell = new TableCell();
            middleCell.CssClass = "tagsMiddle";
            TableCell rightCell = new TableCell();
            rightCell.CssClass = "tagsRight";

            AddValidator.ControlToValidate = AdditionalBox.ID;
            AddValidator.Text = "*";
            AddValidator.EnableClientScript = false;
            
            Controls.Add(labelCell);
            Controls.Add(leftCell);
            Controls.Add(middleCell);
            Controls.Add(rightCell);

            labelCell.Controls.Add(Label);
            leftCell.Controls.Add(AddedList);
            leftCell.Controls.Add(addPanel);
            addPanel.Controls.Add(AdditionalBox);
            addPanel.Controls.Add(AddButton);
            addPanel.Controls.Add(AddValidator);

            middleCell.Controls.Add(TagButton);
            middleCell.Controls.Add(UntagButton);
            rightCell.Controls.Add(AvailableList);
        }

        public IEnumerable<string> GetAddedTags()
        {
            foreach (ListItem item in AddedList.Items)
            {
                yield return item.Value;
            }
        }
    }
}
