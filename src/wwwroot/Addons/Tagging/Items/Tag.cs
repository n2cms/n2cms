using N2.Integrity;
using N2.Templates.Items;
using N2.Web;

namespace N2.Addons.Tagging.Items
{
	[Definition]
	[Template("~/Addons/Tagging/UI/Tag.aspx")]
	[RestrictParents(typeof(TagCategory))]
	public class Tag : AbstractContentPage, ITag
	{
		#region ITag Members

		public int ReferenceCount
		{
			get
			{
				return Find.Items.Where.Detail().Eq(this).Count();
			}
		}

		public ITagCategory Category
		{
			get { return Parent as ITagCategory; }
		}

		#endregion
	}
}