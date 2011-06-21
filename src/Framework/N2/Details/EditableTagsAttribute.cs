using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Persistence;
using N2.Resources;

namespace N2.Details
{
	/// <summary>
	/// Allows editing tags on an item.
	/// </summary>
	/// <example>
	///	// model decoration
	/// [EditableTags(ContainerName = Tabs.Advanced)]
	/// public virtual IEnumerable<string> Tags { get; set; }
	/// 
	/// // locating the tags repository
	/// var repository = N2.Context.Current.Resolve&lt;N2.Persistence.TagsRepository&gt;();
	/// 
	/// // getting tags of an item
	/// var tags = repository.GetTags(item, "Tags");
	/// 
	/// // finding all tags
	/// var tags = repository.FindTags(Find.StartPage, "Tags");
	/// 
	/// // find tagged items
	/// var tags = repository.FindTagged(Find.StartPage, "Tags", "CMS");
	/// </example>
	public class EditableTagsAttribute : EditableTextAttribute
	{
		public EditableTagsAttribute()
		{
			PersistAs = PropertyPersistenceLocation.DetailCollection;
		}

		protected override TextBox CreateEditor()
		{
			var editor = base.CreateEditor();
			editor.CssClass += " tagsEditor";
			return editor;
		}

		protected override Control AddEditor(Control container)
		{
			container.Page.JavaScript("{ManagementUrl}/Resources/Js/EditableTags.js");
			return base.AddEditor(container);
		}

		public override void UpdateEditor(ContentItem item, Control editor)
		{
			var tb = (ITextControl)editor;
			var tags = Engine.Resolve<TagsRepository>().GetTags(item, Name);
			tb.Text = string.Join(", ", tags.ToArray());
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
