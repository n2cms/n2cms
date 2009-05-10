using N2.Integrity;
using N2.Templates.Items;
using N2.Web;

namespace N2.Addons.Tagging.Items
{
	[Definition]
	[Template("~/Addons/Tagging/UI/Tag.aspx")]
	[RestrictParents(typeof(TagGroup))]
	public class Tag : AbstractContentPage, ITag
	{
		public Tag()
		{
			Visible = false;
		}

		#region ITag Members

		public int ReferenceCount
		{
			get
			{
				return Find.Items.Where.Detail().Eq(this).Count();
			}
		}

		public IGroup Category
		{
			get { return Parent as IGroup; }
		}

		#endregion

		public override string IconUrl
		{
			get { return "~/Addons/Tagging/UI/tag_green.png"; }
		}
	}
}