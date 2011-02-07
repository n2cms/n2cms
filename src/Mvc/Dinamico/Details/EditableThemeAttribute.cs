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

		public override void UpdateEditor(ContentItem item, System.Web.UI.Control editor)
		{
			base.UpdateEditor(item, editor);

			if (editor.FindControl(editor.ID + "_preview") != null)
				return;

			var preview = new HyperLink() 
			{ 
				ID = editor.ID + "_preview",
				Text = "Preview", 
				NavigateUrl = "#" 
			};
			preview.Attributes["onclick"] = "window.open('" + N2.Web.Url.Parse(item.Url).AppendQuery("theme", "") + "' + document.getElementById('" + editor.ClientID + "').value, 'previewTheme', 'width=900,height=500'); return false;";
			editor.Parent.Controls.AddAt(editor.Parent.Controls.IndexOf(editor) + 1, preview);
		}
	}
}