using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Web.UI;

namespace N2.Details
{
	public class EditableLanguagesDropDownAttribute : AbstractEditableAttribute
	{
		public EditableLanguagesDropDownAttribute()
		{
		}

		public EditableLanguagesDropDownAttribute(string title, int sortOrder)
			: base(title, sortOrder)
		{
		}

		public override bool UpdateItem(ContentItem item, Control editor)
		{
			DropDownList ddl = editor as DropDownList;
			if (!ddl.SelectedValue.Equals(item[Name] as string, StringComparison.InvariantCultureIgnoreCase))
			{
				item[Name] = ddl.SelectedValue;
				return true;
			}
			return false;
		}

		public override void UpdateEditor(ContentItem item, Control editor)
		{
			DropDownList ddl = editor as DropDownList;
			if (item[Name] != null)
			{
				ddl.SelectedValue = item[Name] as string;
			}
		}

		protected override Control AddEditor(Control container)
		{
			DropDownList ddl = new DropDownList();
			if (!Required)
				ddl.Items.Add(new ListItem());
			foreach (CultureInfo culture in GetCultures())
			{
				ddl.Items.Add(new ListItem(culture.EnglishName, culture.TwoLetterISOLanguageName.ToLowerInvariant()));
			}
			container.Controls.Add(ddl);
			return ddl;
		}

		private static IEnumerable<CultureInfo> GetCultures()
		{	
			Dictionary<string, CultureInfo> cultures = new Dictionary<string, CultureInfo>(StringComparer.Ordinal);
			foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
			{
				RegionInfo region = new RegionInfo(culture.LCID);
				string key = culture.TwoLetterISOLanguageName.ToLowerInvariant();
				if (!cultures.ContainsKey(key))
				{
					cultures.Add(key, culture);
				}
			}

			List < CultureInfo > list = new List<CultureInfo>(cultures.Values);
			list.Sort(delegate(CultureInfo first, CultureInfo second) 
						{ 
							return first.EnglishName.CompareTo(second.EnglishName); 
						});
			return list;
		}
	}
}
