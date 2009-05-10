using System.Collections.Generic;
using N2.Collections;
using N2.Integrity;
using N2.Templates.Items;
using N2.Web;

namespace N2.Addons.Tagging.Items
{
	[Definition]
	[Template("~/Addons/Tagging/UI/TagContainer.aspx")]
	[RestrictParents(typeof(IStructuralPage))]
	public class TagGroup : AbstractContentPage, IGroup
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