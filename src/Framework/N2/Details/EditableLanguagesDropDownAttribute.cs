using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.WebControls;
using System;

namespace N2.Details
{
    /// <summary>
    /// An editable drop down with cultures/languages.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EditableLanguagesDropDownAttribute : EditableDropDownAttribute
    {
        public EditableLanguagesDropDownAttribute()
        {
        }

        public EditableLanguagesDropDownAttribute(string title, int sortOrder)
            : base(title, sortOrder)
        {
        }

        protected override ListItem[] GetListItems()
        {
            List<CultureInfo> cultures = new List<CultureInfo>(CultureInfo.GetCultures(CultureTypes.SpecificCultures));
            cultures.Sort(delegate(CultureInfo first, CultureInfo second)
            {
                return first.DisplayName.CompareTo(second.DisplayName);
            });

            ListItem[] items = new ListItem[cultures.Count];
            for (int i = 0; i < cultures.Count; i++)
            {
                items[i] = new ListItem(cultures[i].DisplayName, cultures[i].Name);
            }
            return items;
        }
    }
}
