using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Persistence;

namespace N2.Details
{
	public class EditableTagsAttribute : EditableTextAttribute
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

		public override void UpdateEditor(ContentItem item, Control editor)
		{
			var tb = (ITextControl)editor;
			var tags = Engine.Resolve<TagsRepository>().GetTags(item, Name);
			tb.Text = string.Join(Environment.NewLine, tags.ToArray());
		}

		public override bool UpdateItem(ContentItem item, Control editor)
		{
			var tb = (ITextControl)editor;
			var rows = tb.Text.Split('\r', '\n', ',').Where(r => !string.IsNullOrEmpty(r)).Select(r => r.Trim()).Distinct(StringComparer.InvariantCultureIgnoreCase).ToList();
			Engine.Resolve<TagsRepository>().SetTags(item, Name, rows);
			return true;
		}
	}
}
