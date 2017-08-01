using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Details
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class EditableRadioListAttribute : AbstractEditableAttribute
    {
        public override bool UpdateItem(N2.ContentItem item, Control editor)
        {
            RadioButtonList rbl = editor as RadioButtonList;
            string itemID = rbl.SelectedValue;
            item[this.Name] = itemID;
            return true;
        }

        public override void UpdateEditor(N2.ContentItem item, Control editor)
        {
            RadioButtonList rbl = editor as RadioButtonList;
            if (rbl != null)
            {
                rbl.SelectedValue = item[this.Name].ToString();
                if (rbl.Items.FindByValue(item[this.Name].ToString()) != null)
                {
                    rbl.Items.FindByValue(item[this.Name].ToString()).Selected = true;
                }
            }
        }

        public override Control AddTo(Control container)
        {
            Control panel = AddPanel(container);
            AddLabel(panel);
            Control ddl = AddRadiolist(panel);
            if (Validate)
                AddRegularExpressionValidator(panel, ddl);
            if (Required)
                AddRequiredFieldValidator(panel, ddl);
            return ddl;
        }

        private Control AddRadiolist(Control panel)
        {
            RadioButtonList rbl = new RadioButtonList();
            foreach (ListItem li in GetListItems(panel))
            {
                rbl.Items.Add(li);
                rbl.CellPadding = 5;
            }
            rbl.ID = Name;
            rbl.RepeatDirection = RepeatDirection.Horizontal;
            rbl.RepeatLayout = RepeatLayout.Table;
            panel.Controls.Add(rbl);
            return rbl;
        }

        protected override Control AddEditor(Control container)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected abstract IEnumerable<ListItem> GetListItems(Control container);
    }
}
