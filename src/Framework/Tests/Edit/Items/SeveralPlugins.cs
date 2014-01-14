using N2.Edit;
using N2.Security;

[assembly: ToolbarPlugin("Don't worry be happy", "peace", "/lsd.aspx", ToolbarArea.Navigation, SortOrder=10)]
[assembly: NavigationLinkPlugin("Buzz out", "buzz", "~/alarm.aspx", Targets.Preview, "/icons/buzz.gif", 40)]
namespace N2.Tests.Edit.Items
{
    [ToolbarPlugin("Worry we're coming", "panic", "/run.aspx", ToolbarArea.Preview, SortOrder = 20, AuthorizedRoles = new string[] { "ÜberEditor" })]
    [NavigationLinkPlugin("Chill in", "chill", "~/freeze.aspx", Targets.Preview, "/icons/chill.gif", 30, AuthorizedRoles = new string[] { "ÜberEditor" })]
    public class SeveralPlugins
    {
    }
    [ToolbarPlugin("Worry we're coming 2", "panic2", "/run2.aspx", ToolbarArea.Preview, SortOrder = 50, RequiredPermission = Permission.Administer)]
    [NavigationLinkPlugin("Chill in 2", "chill2", "~/freeze2.aspx", Targets.Preview, "/icons/chill.gif", 50, RequiredPermission = Permission.Administer)]
    public class PermittablePlugins
    {
    }
}
