using N2.Definitions;
using N2.Edit.Trash;
using N2.Integrity;

namespace N2.Templates.Mvc.Items.Items
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