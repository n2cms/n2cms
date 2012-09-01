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
	[AttributeUsage(AttributeTargets.Property)]
	public class EditableEnumAttribute : EditableDropDownAttribute
    {
		public EditableEnumAttribute()
			: this("", 10, typeof(EmptyEnum))
		{
		}

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
            EnumType = enumType;
        }

		/// <summary>The type of enum listed by this editor.</summary>
		public Type EnumType { get; set; }

        protected override System.Web.UI.WebControls.ListItem[] GetListItems()
        {
            Array values = Enum.GetValues(EnumType);
            ListItem[] items = new ListItem[values.Length];
            for (int i = 0; i < values.Length; i++)
			{
                int value = (int)values.GetValue(i);
                string name = Utility.GetGlobalResourceString(EnumType.Name, Enum.GetName(EnumType, value)) 
                    ?? Enum.GetName(EnumType, value);
                items[i] = new ListItem(name, value.ToString());
            }
            return items;
        }

        protected override string GetValue(ContentItem item)
        {
            object value = item[Name];

            return ConvertToString(value) ?? ConvertToString(DefaultValue);
        }

        protected override string ConvertToString(object value)
        {
            if (value == null)
                return null;

            if (value is string)
                // an enum as string we assume
                return ((int)Enum.Parse(EnumType, (string)value)).ToString();

            if (value is int)
                // an enum as int we hope
                return value.ToString();

            // hopefully an enum type;
            return ((int)value).ToString();
        }

        protected override object ConvertToValue(string value)
        {
            return Enum.ToObject(EnumType, int.Parse(value));
        }

		enum EmptyEnum
		{
		}
    }
}
