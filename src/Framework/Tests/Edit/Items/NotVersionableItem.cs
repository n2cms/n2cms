using N2.Definitions;
using N2.Persistence;

namespace N2.Tests.Edit.Items
{
    [Versionable(AllowVersions.No)]
    public class NotVersionableItem : ContentItem
    {
    }
    [NotVersionable]
    public class LegacyNotVersionableItem : ContentItem
    {
    }
}
