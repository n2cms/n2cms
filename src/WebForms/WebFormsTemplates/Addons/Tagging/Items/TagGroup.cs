using System.Collections.Generic;
using N2.Collections;
using N2.Details;
using N2.Persistence;

namespace N2.Addons.Tagging.Items
{
    [PageDefinition("Tag Group",
        Description = "Define a tag group that pages can be associated to.",
        TemplateUrl = "~/Addons/Tagging/UI/TagContainer.aspx",
        IconUrl = "~/Addons/Tagging/UI/tag_red.png", 
        SortOrder = 550)]
    [WithEditableName, WithEditableTitle]
    public class TagGroup : ContentItem, IGroup
    {
        public TagGroup()
        {
            Visible = false;
        }

        #region ITagContainer Members
        
        public IEnumerable<ITag> GetTags()
        {
            foreach(Tag t in GetChildren(new TypeFilter(typeof (Tag))))
                yield return t;
        }

        public ITag GetOrCreateTag(string tagName)
        {
            foreach(ContentItem child in Children)
            {
                if(child.Title == tagName)
                    return child as Tag;
            }

            var tag = N2.Context.Current.Resolve<ContentActivator>().CreateInstance<Tag>(this);
            tag.Name = tagName;
            tag.Title = tagName;
            N2.Context.Current.Persister.Save(tag);
            return tag;
        }

        #endregion
    }
}
