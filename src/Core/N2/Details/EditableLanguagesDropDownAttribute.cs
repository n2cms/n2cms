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
			foreach (RegionInfo culture in GetCultures())
			{
				ddl.Items.Add(new ListItem(culture.DisplayName, culture.TwoLetterISORegionName));
			}
			container.Controls.Add(ddl);
			return ddl;
		}

		private static IEnumerable<RegionInfo> GetCultures()
		{
			Dictionary<string, RegionInfo> cultures = new Dictionary<string, RegionInfo>(StringComparer.InvariantCultureIgnoreCase);
			foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
			{
				RegionInfo region = new RegionInfo(culture.LCID);
				string key = region.TwoLetterISORegionName;
				if (!cultures.ContainsKey(key))
				{
					cultures.Add(key, region);
				}
			}

			List<RegionInfo> list = new List<RegionInfo>(cultures.Values);
			list.Sort(delegate(RegionInfo first, RegionInfo second) 
						{
							return first.DisplayName.CompareTo(second.DisplayName); 
						});
			return list;
		}
	}
}
