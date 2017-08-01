using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Details
{
    public class EditableBooleanAttribute : EditableRadioListAttribute
    {
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
