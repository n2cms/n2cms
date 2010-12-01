using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Resources;
using System.Text;

namespace N2.Details
{
	/// <summary>
	/// Allows the selection of themes.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class EditableThemeSelectionAttribute : EditableListControlAttribute
	{
		public EditableThemeSelectionAttribute(string title, int sortOrder)
			: base(title, sortOrder)
		{
		}

		protected override Control AddEditor(Control container)
		{
			var editor = base.AddEditor(container);
			Register.JavaScript(container.Page, Engine.ManagementPaths.ResolveResourceUrl("{ManagementUrl}/Resources/Js/jquery.editableThemeSelection.js"));
			
			StringBuilder initializationScript = new StringBuilder();
			initializationScript.AppendFormat("jQuery('#{0}').editableThemeSelection({{ '' : null", editor.ClientID);
			foreach (string directoryPath in GetThemeDirectories())
			{
				string thumbnailPath = Path.Combine(directoryPath, "thumbnail.jpg");
				string themeName = Path.GetFileName(directoryPath);
				if (File.Exists(thumbnailPath))
				{
					initializationScript.AppendFormat(", '{0}' : '{1}'", 
						themeName, 
						container.ResolveClientUrl(string.Format("~/App_Themes/{0}/thumbnail.jpg", themeName)));
				}
				else
				{
					initializationScript.AppendFormat(", '{0}' : null", 
						themeName);
				}
			}
			initializationScript.AppendFormat("}});");
			Register.JavaScript(container.Page, initializationScript.ToString(), ScriptOptions.DocumentReady);

			return editor;
		}

		protected override ListControl CreateEditor()
		{
			return new DropDownList();
		}

		protected override ListItem[] GetListItems()
		{
			List<ListItem> items = new List<ListItem>();

			foreach (string directoryPath in GetThemeDirectories())
			{
				string directoryName = Path.GetFileName(directoryPath);
				if (!directoryName.StartsWith("."))
					items.Add(new ListItem(directoryName));
			}

			return items.ToArray();
		}

		private IEnumerable<string> GetThemeDirectories()
		{
			string path = HostingEnvironment.MapPath("~/App_Themes/");
			if (Directory.Exists(path))
			{
				foreach (string directoryPath in Directory.GetDirectories(path))
				{
					string directoryName = Path.GetFileName(directoryPath);
					if (!directoryName.StartsWith("."))
						yield return directoryPath;
				}
			}
		}
	}
}
