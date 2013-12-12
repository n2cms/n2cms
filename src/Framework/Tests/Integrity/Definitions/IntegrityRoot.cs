using N2.Integrity;

namespace N2.Tests.Integrity.Definitions
{
    [PageDefinition]
    [AllowedChildren(typeof(IntegrityStartPage))]
    public class IntegrityRoot : N2.ContentItem
    {
    }
}
