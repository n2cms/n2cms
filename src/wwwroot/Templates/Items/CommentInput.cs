using N2.Integrity;

namespace N2.Templates.Items
{
    [Definition("Comment Input Form")]
    [RestrictParents(typeof(AbstractPage))]
    [AllowedZones(Zones.Content, Zones.RecursiveBelow)]
    public class CommentInput : AbstractItem
    {
        protected override string IconName { get { return "comment_add"; } }
    }
}
