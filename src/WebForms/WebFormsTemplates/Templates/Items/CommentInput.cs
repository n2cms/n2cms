using N2.Integrity;

namespace N2.Templates.Items
{
    [PartDefinition("Comment Input Form",
        IconUrl = "~/Templates/UI/Img/comment_add.png")]
    [RestrictParents(typeof(AbstractPage))]
    [AllowedZones(Zones.Content, Zones.RecursiveBelow)]
    public class CommentInput : AbstractItem
    {
    }
}
