using System;
using System.Collections.Generic;
using System.Text;
using N2.Edit;

[assembly: ToolbarPlugin("Don't worry be happy", "peace", "/lsd.aspx", ToolbarArea.Navigation, SortOrder=10)]
[assembly: NavigationPlugin("Buzz out", "buzz", "~/alarm.aspx", "preview", "/icons/buzz.gif", 40)]
namespace N2.Tests.Edit.Items
{
	[ToolbarPlugin("Worry we're coming", "panic", "/run.aspx", ToolbarArea.Preview, SortOrder=20)]
	[NavigationPlugin("Chill in", "chill", "~/freeze.aspx", "preview", "/icons/chill.gif", 30)]
	[N2.Edit.PlugInAuthorizedRoles("ÜberEditor")]
	public class SeveralPlugins
	{
	}
}
