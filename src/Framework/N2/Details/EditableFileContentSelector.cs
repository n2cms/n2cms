using N2.Details;
using N2.Edit.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;

namespace N2.Details
{
	public class EditableFileContentSelector : EditableDropDownAttribute
	{
		public string FileVirtualPath { get; set; }

		public string FileContentSelectorExpression { get; set; }

		protected override ListItem[] GetListItems()
		{
			var fs = Engine.Resolve<IFileSystem>();
			if (!fs.FileExists(FileVirtualPath))
                return new ListItem[0];

			using (var s = fs.OpenFile(FileVirtualPath, readOnly: true))
			using (var sr = new StreamReader(s))
			{
				var content = sr.ReadToEnd();
                var matches = Regex.Matches(content, FileContentSelectorExpression);
				return matches.OfType<Match>()
					.Select(m => new ListItem(m.Groups.Count > 1 ? m.Groups[1].Value : m.Value))
					.ToArray();
			}
		}
	}
}