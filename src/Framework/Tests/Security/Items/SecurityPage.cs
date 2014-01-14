using N2.Security;

namespace N2.Tests.Security.Items
{
    [N2.PageDefinition("Page")]
    public class SecurityPage : N2.ContentItem
    {
    }

    [N2.PageDefinition("RemappedPage"), N2.Security.PermissionRemap(From = Permission.Publish, To = Permission.Write)]
    public class RemappedPage : N2.ContentItem
    {
    }
}
