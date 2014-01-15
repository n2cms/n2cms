using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Details;

namespace N2.Templates.Details
{
    public abstract class DropDownAttribute : AbstractEditableAttribute
    {
        public DropDownAttribute(string title, string name, int sortOrder)
            :base(title, name, sortOrder)
        {
        }

        public override bool UpdateItem(ContentItem item, Control editor)
        {
            DropDownList ddl = (DropDownList)editor;
            if (ddl.SelectedValue != item[Name] as string)
            {
                item[Name] = ddl.SelectedValue;
                return true;
            }
            return false;
        }

        public override void UpdateEditor(ContentItem item, Control editor)
        {
            DropDownList ddl = (DropDownList)editor;
            if (ddl.Items.FindByValue(item[Name] as string) != null)
                ddl.SelectedValue = item[Name] as string;
        }

        protected override Control AddEditor(Control container)
        {
            DropDownList ddl = new DropDownList();
            foreach (ListItem li in GetListItems(container))
            {
                ddl.Items.Add(li);
            }
            container.Controls.Add(ddl);
            return ddl;
        }

        protected abstract IEnumerable<ListItem> GetListItems(Control container);
    }
}
