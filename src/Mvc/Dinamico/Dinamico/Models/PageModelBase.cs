using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2;
using N2.Details;
using N2.Web.UI;

namespace Dinamico.Models
{
	[WithEditableTitle]
	[WithEditableName(ContainerName = Defaults.Containers.Metadata)]
	[WithEditableVisibility(ContainerName = Defaults.Containers.Metadata)]
	[SidebarContainer(Defaults.Containers.Metadata, 100, HeadingText = "Metadatda")]
	[TabContainer(Defaults.Containers.Content, "Content", 1000)]
	public abstract class PageModelBase : ContentItem
	{
	}
}