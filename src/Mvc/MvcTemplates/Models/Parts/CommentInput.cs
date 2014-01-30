using N2.Integrity;
using N2.Templates.Mvc.Models.Pages;
using N2.Definitions;

namespace N2.Templates.Mvc.Models.Parts
{
    [PartDefinition("Comment Input Form",
        IconUrl = "~/Content/Img/comment_add.png")]
    [RestrictParents(typeof (ICommentable))]
    [AllowedZones(Zones.Content, Zones.RecursiveBelow)]
    [RestrictCardinality]
    public class CommentInput : PartBase
    {
    }
}
