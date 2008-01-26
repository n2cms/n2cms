using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Templates.Details
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ThemeSelectorAttribute : DropDownAttribute
	{
		public ThemeSelectorAttribute(string title, int sortOrder)
			:base(title, null, sortOrder)
		{
		}

		protected override IEnumerable<ListItem> GetListItems(Control container)
		{
			string path = container.Page.Server.MapPath("~/App_Themes/");

			yield return new ListItem();
			foreach(string file in Directory.GetDirectories(path))
			{
				string fileName = Path.GetFileName(file);
				yield return new ListItem(fileName);
			}
		}
	}
}
