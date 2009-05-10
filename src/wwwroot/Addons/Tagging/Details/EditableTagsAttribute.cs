using System.Collections.Generic;
using N2.Addons.Tagging.Details.WebControls;
using N2.Addons.Tagging.Items;
using N2.Collections;
using N2.Details;
using System.Web.UI;
using System;

namespace N2.Addons.Tagging.Details
{
	public class EditableTagsAttribute : AbstractEditableAttribute
	{
		public EditableTagsAttribute()
		{
			TagGroupName = "Tags";
		}
		public string TagGroupName { get; set; }

		public override bool UpdateItem(ContentItem item, Control editor)
		{
			TagsEditor tagEditor = editor as TagsEditor;
			if(tagEditor.HasChanges)
			{
				IList<Items.TagCategory> containers = GetAvailableCategories(item);
				IEnumerable<AppliedTags> changes = tagEditor.GetAddedTags(containers);
				foreach(AppliedTags change in changes)
				{
					ApplyChanges(change, item);
				}
			}

			return tagEditor.HasChanges;
		}

		private void ApplyChanges(AppliedTags change, ContentItem item)
		{
			DetailCollection links = item.GetDetailCollection(Name, false);
			if (links == null)
			{
				if (change.Tags.Count == 0)
					return;
				links = item.GetDetailCollection(Name, true);
			}

			List<ITag> currentTags = GetCurrentTags(change.Category, links);

			IEnumerable<string> addedTags = GetAddedTags(currentTags, change.Tags);
			foreach(string tagName in addedTags)
			{
				ITag tag = change.Category.GetOrCreateTag(tagName);
				links.Add(tag);
			}
			
			foreach(ContentItem tag in currentTags)
			{
				if (!change.Tags.Contains(tag.Title))
                    links.Remove(tag);
			}
		}

		List<ITag> GetCurrentTags(ITagCategory category, DetailCollection links)
		{
			List<ITag> tags = new List<ITag>();
			foreach (ContentItem link in links)
			{
				if(link.Parent == category)
					tags.Add(link as ITag);
			}
			return tags;
		}

		private IEnumerable<string> GetAddedTags(List<ITag> currentTags, IList<string> addedTags)
		{
			List<string> tagsToAdd = new List<string>();
			foreach(string tagName in addedTags)
			{
				bool alreadyAdded = currentTags.Exists(t => t.Title == tagName);
				if (!alreadyAdded)
					tagsToAdd.Add(tagName);
			}
			return tagsToAdd;
		}

		public override void UpdateEditor(ContentItem item, Control editor)
		{
			TagsEditor tagEditor = editor as TagsEditor;

			IList<Items.TagCategory> containers = GetAvailableCategories(item);

			if(containers != null)
			{
				List<AppliedTags> changes = GetChanges(item, containers);
				tagEditor.BindTo(changes);
			}
		}

		List<AppliedTags> GetChanges(ContentItem item, IEnumerable<TagCategory> containers)
		{
			List<AppliedTags> selections = new List<AppliedTags>();
			foreach(Items.TagCategory container in containers)
			{
				IEnumerable<string> tags = GetSelectedTags(item, container);
				selections.Add(new AppliedTags
				{
					Category = container,
					Tags = new List<string>(tags)
				});
			}
			return selections;
		}

		IEnumerable<string> GetSelectedTags(ContentItem item, ITagCategory category)
		{
			DetailCollection links = item.GetDetailCollection(Name, false);
			if (links != null)
			{
				foreach (ContentItem link in links)
					if(link.Parent == category)
						yield return link.Title;
			}
		}

		private IList<TagCategory> GetAvailableCategories(ContentItem item)
		{
			foreach (var ancestor in Find.EnumerateParents(item, null, true))
			{
				ItemList tagContainers = ancestor.GetChildren(new TypeFilter(typeof(Items.TagCategory)));
				if (tagContainers.Count > 0)
					return tagContainers.Cast<TagCategory>();
			}
			return null;
		}

		public override Control AddTo(Control container)
		{
			return AddEditor(container);
		}

		protected override Control AddEditor(Control container)
		{
			TagsEditor t = new TagsEditor();
			container.Controls.Add(t);
			return t;
		}
	}
}
