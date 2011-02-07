using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Details;
using System.Web.Hosting;
using System.Web.UI.WebControls;

namespace N2.Details
{
	public class EditableThemeAttribute : EditableDropDownAttribute
	{
		public EditableThemeAttribute()
		{
			DefaultValue = "Default";
			Required = true;
		}

		protected override ListItem[] GetListItems()
		{
			List<ListItem> items = new List<ListItem> { new ListItem("", "Default") };

			var dir = HostingEnvironment.VirtualPathProvider.GetDirectory("~/Themes/");
			if (dir != null)
			{
				items.AddRange(dir.Directories.OfType<VirtualDirectory>()
					.Where(d => !d.Name.Equals("Default", StringComparison.InvariantCultureIgnoreCase))
					.Select(d => new ListItem(d.Name)));
			}

			return items.ToArray();
		}
	}
}