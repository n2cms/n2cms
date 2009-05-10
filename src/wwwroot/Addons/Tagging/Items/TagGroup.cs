using System.Collections.Generic;
using N2.Collections;
using N2.Details;
using N2.Web;

namespace N2.Addons.Tagging.Items
{
	[Definition("Tag Group", Description = "Define a tag group that pages can be associated to.", SortOrder = 550)]
	[Template("~/Addons/Tagging/UI/TagContainer.aspx")]
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

			var tag = N2.Context.Current.Definitions.CreateInstance<Tag>(this);
			tag.Name = tagName;
			tag.Title = tagName;
			N2.Context.Current.Persister.Save(tag);
			return tag;
		}

		#endregion

		public override string IconUrl
		{
			get { return "~/Addons/Tagging/UI/tag_red.png"; }
		}
	}
}