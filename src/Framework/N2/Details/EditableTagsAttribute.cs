using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Persistence;

namespace N2.Details
{
	internal class EditableTagsAttribute : EditableTextAttribute
	{
		public EditableTagsAttribute()
		{
			TextMode = TextBoxMode.MultiLine;
			PersistAs = PropertyPersistenceLocation.DetailCollection;
		}

		protected override TextBox CreateEditor()
		{
			var editor = base.CreateEditor();
			editor.CssClass += " tagsEditor";
			return editor;
		}
	}
}
