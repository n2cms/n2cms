using System;
using N2;
using N2.Definitions;
using N2.Details;
using N2.Integrity;
using N2.Web.UI;

namespace Dinamico.Models
{
	/// <summary>
	///     Base implementation for pages on a dinamico site.
	/// </summary>
	[WithEditableTitle]
	[WithEditableName(ContainerName = Defaults.Containers.Metadata)]
	[WithEditableVisibility(ContainerName = Defaults.Containers.Metadata)]
	[SidebarContainer(Defaults.Containers.Metadata, 100, HeadingText = "Metadata")]
	[TabContainer(Defaults.Containers.Content, "Content", 1000)]
	[RestrictParents(typeof(IPage))]
	public abstract class PageModelBase : ContentItem, IPage
	{
	}
}