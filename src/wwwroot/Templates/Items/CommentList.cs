using N2.Definitions;
using N2.Edit.Trash;
using N2.Integrity;

namespace N2.Templates.Items
{
    [Disable] // This item is added by the CommentInput thus it's disabled
    [NotThrowable]
	[PartDefinition("Comment List",
		IconUrl = "~/Templates/UI/Img/comments.png")]
    [RestrictParents(typeof(AbstractPage))]
    public class CommentList : AbstractItem
    {
    }
}
