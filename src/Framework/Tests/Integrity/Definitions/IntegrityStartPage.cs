using N2.Integrity;

namespace N2.Tests.Integrity.Definitions
{
    [PageDefinition]
    [RestrictParents(AllowedTypes.None)] // no parents allowed
    public class IntegrityStartPage : N2.ContentItem
    {
    }
}
