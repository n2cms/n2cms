using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Persistence;
using N2.Resources;
using N2.Persistence.Search;
using N2.Web;

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
	[AttributeUsage(AttributeTargets.Property)]
	public class EditableTagsAttribute : EditableTextAttribute
	{
		public EditableTagsAttribute()
		{
			PersistAs = PropertyPersistenceLocation.DetailCollection;
			AutocompleteUrl = "{ManagementUrl}/tags.n2.ashx";
		}

		public string AutocompleteUrl { get; set; }

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
			var tb = (TextBox)editor;
			tb.Text = GetTagsText(item);


			tb.Attributes["data-autocomplete-url"] = AutocompleteUrl.ToUrl()
				.AppendQuery("tagName", Name)
				.AppendQuery("action=tags")
				.AppendQuery("selected", item.Path)
				.ResolveTokens();
			tb.Attributes["data-selected"] = item.Path;
		}

		public override bool UpdateItem(ContentItem item, Control editor)
		{
			var tb = (ITextControl)editor;
			var rows = tb.Text.Split('\r', '\n', ',').Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToList();
			Engine.Resolve<TagsRepository>().SetTags(item, Name, rows);
			return true;
		}

		public override string GetIndexableText(ContentItem item)
		{
			return GetTagsText(item);
		}

		public override void Write(ContentItem item, string propertyName, System.IO.TextWriter writer)
		{
			writer.Write(GetTagsText(item));
		}

		private string GetTagsText(ContentItem item)
		{
			var tags = TagsRepository.GetTagsFromValues(item, Name);
			return string.Join(", ", tags.ToArray());
		}
	}
}
