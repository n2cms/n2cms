using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2;
using N2.Details;
using N2.Web.UI;

namespace RavenTest.Models
{
	[PageDefinition]
	[WithEditableTitle]
	[WithEditableName(ContainerName = "Sidebar")]
	[SidebarContainer("Sidebar", 100)]
	public class Page : ContentItem
	{
	}
}