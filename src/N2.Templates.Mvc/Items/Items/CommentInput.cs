using N2.Integrity;

namespace N2.Templates.Mvc.Items.Items
{
	[PartDefinition("Comment Input Form",
		IconUrl = "~/Content/Img/comment_add.png")]
	[RestrictParents(typeof (AbstractPage))]
	[AllowedZones(Zones.Content, Zones.RecursiveBelow)]
	public class CommentInput : AbstractItem
	{
	}
}