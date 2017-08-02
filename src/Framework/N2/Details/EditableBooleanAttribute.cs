using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Details
{
    public class EditableBooleanAttribute : EditableRadioListAttribute
    {
        public override bool UpdateItem(N2.ContentItem item, Control editor)
        {
            var rbl = editor as RadioButtonList;
            var tf = Convert.ToBoolean(rbl.SelectedValue);
            item[this.Name] = tf;

            return true;
        }
       
        protected override IEnumerable<ListItem> GetListItems(Control container)
        {
            return new ListItem[]
            {
                new ListItem("Yes", Boolean.TrueString),
                new ListItem("No", Boolean.FalseString)
            };
        }
    }
}
