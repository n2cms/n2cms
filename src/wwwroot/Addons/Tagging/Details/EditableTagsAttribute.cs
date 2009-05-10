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
			TagsTable tagEditor = editor as TagsTable;
			if(tagEditor.HasChanges)
			{
				IList<Items.TagCategory> containers = GetTagContainer(item);
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
			DetailCollection links = item.GetDetailCollection(change.Category.Name, false);
			if (links == null)
			{
				if (change.Tags.Count == 0)
					return;
				links = item.GetDetailCollection(change.Category.Name, true);
			}

			ContentItem[] currentTags = links.ToArray<ContentItem>();
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

		private IEnumerable<string> GetAddedTags(ContentItem[] currentTags, IList<string> addedTags)
		{
			List<string> tagsToAdd = new List<string>();
			foreach(string tagName in addedTags)
			{
				bool alreadyAdded = Array.Exists(currentTags, x => x.Title == tagName);
				if (!alreadyAdded)
					tagsToAdd.Add(tagName);
			}
			return tagsToAdd;
		}

		public override void UpdateEditor(ContentItem item, Control editor)
		{
			TagsTable tagEditor = editor as TagsTable;

			IList<Items.TagCategory> containers = GetTagContainer(item);

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
				IEnumerable<string> tags = GetSelectedTags(item, container.Name);
				selections.Add(new AppliedTags
				{
					Category = container,
					Tags = new List<string>(tags)
				});
			}
			return selections;
		}

		IEnumerable<string> GetSelectedTags(ContentItem item, string name)
		{
			DetailCollection links = item.GetDetailCollection(name, false);
			if (links != null)
			{
				foreach (ContentItem link in links)
					yield return link.Title;
			}
		}

		private IList<Items.TagCategory> GetTagContainer(ContentItem item)
		{
			foreach (var ancestor in Find.EnumerateParents(item, null, true))
			{
				ItemList tagContainers = ancestor.GetChildren(new TypeFilter(typeof(Items.TagCategory)));
				if (tagContainers.Count > 0)
					return tagContainers.Cast<Items.TagCategory>();
			}
			return null;
		}

		public override Control AddTo(Control container)
		{
			return AddEditor(container);
		}

		protected override Control AddEditor(Control container)
		{
			TagsTable t = new TagsTable();
			container.Controls.Add(t);
			return t;
		}
	}
}
