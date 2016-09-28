using System;
using N2;
using N2.Definitions;
using N2.Web.UI;

namespace Dinamico.Models
{
	/// <summary>
	///     Base implementation of parts on a dinamico site.
	/// </summary>
	[SidebarContainer(Defaults.Containers.Metadata, 100, HeadingText = "Metadata")]
	public abstract class PartModelBase : ContentItem, IPart
	{
	}
}