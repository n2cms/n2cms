using System;
using System.Collections.Generic;
using System.Text;
using N2.Edit;

[assembly: ToolbarPlugIn("Don't worry be happy", "peace", "/lsd.aspx", ToolbarArea.Navigation, SortOrder=10)]
[assembly: NavigationPlugIn("Buzz out", "buzz", "~/alarm.aspx", "preview", "/icons/buzz.gif", 40)]
namespace N2.Tests.Edit.Items
{
	[ToolbarPlugIn("Worry we're coming", "panic", "/run.aspx", ToolbarArea.Preview, SortOrder=20)]
	[NavigationPlugIn("Chill in", "chill", "~/freeze.aspx", "preview", "/icons/chill.gif", 30)]
	[N2.Edit.PlugInAuthorizedRoles("ÜberEditor")]
	public class SeveralPlugins
	{
	}
}
