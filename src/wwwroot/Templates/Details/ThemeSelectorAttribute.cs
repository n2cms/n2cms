using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
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
			string path = HostingEnvironment.MapPath("~/App_Themes/");

			yield return new ListItem();
			foreach(string directoryPath in Directory.GetDirectories(path))
			{
				string directoryName = Path.GetFileName(directoryPath);
				if(!directoryName.StartsWith("."))
					yield return new ListItem(directoryName);
			}
		}
	}
}
