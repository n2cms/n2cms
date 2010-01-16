using N2.Integrity;
using N2.Templates.Mvc.Items;

namespace N2.Templates.Mvc.Models.Parts
{
	[PartDefinition("Comment Input Form",
		IconUrl = "~/Content/Img/comment_add.png")]
	[RestrictParents(typeof (AbstractPage))]
	[AllowedZones(Zones.Content, Zones.RecursiveBelow)]
	public class CommentInput : AbstractItem
	{
	}
}