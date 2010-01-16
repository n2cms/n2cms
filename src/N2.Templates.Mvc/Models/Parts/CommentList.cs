using N2.Definitions;
using N2.Edit.Trash;
using N2.Integrity;
using N2.Templates.Mvc.Items;

namespace N2.Templates.Mvc.Models.Parts
{
	[Disable] // This item is added by the CommentInput thus it's disabled
	[NotThrowable]
	[PartDefinition("Comment List",
		IconUrl = "~/Content/Img/comments.png")]
	[RestrictParents(typeof (AbstractPage))]
	public class CommentList : AbstractItem
	{
	}
}