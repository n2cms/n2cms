using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace N2.Details
{
    public class EditableEnumAttribute : EditableDropDownAttribute
    {
        Type enumType;
        public EditableEnumAttribute(string title, int sortOrder, Type enumType)
            : base(title, sortOrder)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new ArgumentException("The parameter 'enumType' is not a type of enum.", "enumType");
            
            Required = true;
            this.enumType = enumType;
        }

        protected override System.Web.UI.WebControls.ListItem[] GetListItems()
        {
            Array values = Enum.GetValues(enumType);
            ListItem[] items = new ListItem[values.Length];
            for (int i = 0; i < values.Length; i++)
			{
                int value = (int)values.GetValue(i);
			    string name = Enum.GetName(enumType, value);
                items[i] = new ListItem(name, value.ToString());
            }
            return items;
        }

        protected override string GetValue(ContentItem item)
        {
            if (item[Name] != null)
                return ((int)item[Name]).ToString();
            else
                return null;
        }

        protected override object GetValue(DropDownList ddl)
        {
            if (!string.IsNullOrEmpty(ddl.SelectedValue))
                return Enum.Parse(enumType, ddl.SelectedValue);
            else
                return null;
        }
    }
}
