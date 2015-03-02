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
                IList<Items.TagGroup> containers = GetAvailableCategories(item);
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

            List<ITag> currentTags = GetCurrentTags(change.Group, links);

            IEnumerable<string> addedTags = GetAddedTags(currentTags, change.Tags);
            foreach(string tagName in addedTags)
            {
                ITag tag = change.Group.GetOrCreateTag(tagName);
                links.Add(tag);
            }
            
            foreach(ContentItem tag in currentTags)
            {
                if (!change.Tags.Contains(tag.Title))
                    links.Remove(tag);
            }
        }

        List<ITag> GetCurrentTags(IGroup group, DetailCollection links)
        {
            List<ITag> tags = new List<ITag>();
            foreach (ContentItem link in links)
            {
                if(link.Parent != null && link.Parent.Equals(group))
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

            IList<Items.TagGroup> containers = GetAvailableCategories(item);

            if(containers != null)
            {
                List<AppliedTags> changes = GetChanges(item, containers);
                tagEditor.BindTo(changes);
            }
        }

        List<AppliedTags> GetChanges(ContentItem item, IEnumerable<TagGroup> containers)
        {
            List<AppliedTags> selections = new List<AppliedTags>();
            foreach(Items.TagGroup group in containers)
            {
                IEnumerable<string> tags = GetSelectedTags(item, group);
                selections.Add(new AppliedTags
                {
                    Group = group,
                    Tags = new List<string>(tags)
                });
            }
            return selections;
        }

        IEnumerable<string> GetSelectedTags(ContentItem item, IGroup group)
        {
            DetailCollection links = item.GetDetailCollection(Name, false);
            if (links != null)
            {
                foreach (ContentItem link in links)
                    if(link.Parent != null && link.Parent.Equals(group))
                        yield return link.Title;
            }
        }

        private IList<TagGroup> GetAvailableCategories(ContentItem item)
        {
            foreach (var ancestor in Find.EnumerateParents(item, null, true))
            {
                ItemList tagContainers = ancestor.GetChildPagesUnfiltered().Where(new TypeFilter(typeof(Items.TagGroup)));
                if (tagContainers.Count > 0)
                    return tagContainers.Cast<TagGroup>();
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
            t.ID = Name;
            container.Controls.Add(t);
            return t;
        }
    }
}
