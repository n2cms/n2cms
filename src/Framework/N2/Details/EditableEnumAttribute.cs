using System;
using System.Web.UI.WebControls;

namespace N2.Details
{
    /// <summary>
    /// Adds a drop down for editing values in an enum attribute.
    /// </summary>
    /// <remarks>
    /// Depending on the property type the values will be stored as enum, integer or string.
    /// </remarks>
    public class EditableEnumAttribute : EditableDropDownAttribute
    {
        Type enumType;

		public EditableEnumAttribute(Type enumType)
			: this("", 10, enumType)
		{
		}

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
                string name = Utility.GetGlobalResourceString(enumType.Name, Enum.GetName(enumType, value)) 
                    ?? Enum.GetName(enumType, value);
                items[i] = new ListItem(name, value.ToString());
            }
            return items;
        }

        protected override string GetValue(ContentItem item)
        {
            object value = item[Name];
            
            if(value == null)
                return null;

            if (value is string)
                // an enum as string we assume
                return ((int)Enum.Parse(enumType, (string)value)).ToString();
            
            if (value is int)
                // an enum as int we hope
                return value.ToString();
            
            // hopefully an enum type;
            return ((int) value).ToString();
        }

        protected override object GetValue(ListControl ddl)
        {
            if (!string.IsNullOrEmpty(ddl.SelectedValue))
                return GetEnumValue(int.Parse(ddl.SelectedValue));
            else
                return null;
        }

        private object GetEnumValue(int value)
        {
            foreach (object e in Enum.GetValues(enumType))
            {
                if ((int)e == value)
                    return e;
            }
            return null;
        }
    }
}
