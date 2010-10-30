using N2.Integrity;
using N2.Templates.Mvc.Models.Pages;

namespace N2.Templates.Mvc.Models.Parts
{
	[PartDefinition("Comment Input Form",
		IconUrl = "~/Content/Img/comment_add.png")]
	[RestrictParents(typeof (PageBase))]
	[AllowedZones(Zones.Content, Zones.RecursiveBelow)]
	public class CommentInput : PartBase
	{
	}
}