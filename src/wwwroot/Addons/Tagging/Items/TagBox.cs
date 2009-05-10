using N2.Integrity;
using N2.Web;

namespace N2.Addons.Tagging.Items
{
	[Definition]
	[Template("~/Addons/Tagging/UI/TagBox.ascx")]
	[AllowedZones(AllowedZones.AllNamed)]
	public class TagBox : ContentItem
	{
		public override string IconUrl
		{
			get { return "~/Addons/Tagging/UI/tag_yellow.png"; }
		}
		public override bool IsPage
		{
			get { return false; }
		}
	}
}
