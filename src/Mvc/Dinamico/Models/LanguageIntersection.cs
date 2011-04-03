using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2;
using N2.Integrity;
using N2.Definitions;
using N2.Details;
using N2.Web;
using N2.Web.UI;

namespace Dinamico.Models
{
	[PageDefinition(
		IconUrl = "{IconsUrl}/world_go.png",
		InstallerVisibility = N2.Installation.InstallerHint.PreferredStartPage)]
	[RestrictParents(typeof(IRootPage))]
	[TabContainer(Defaults.Containers.Site, "Site", 1000)]
	public class LanguageIntersection : PageModelBase, IThemeable, ISitesSource
	{
		#region IThemeable Members

		[EditableThemeAttribute]
		public virtual string Theme { get; set; }

		#endregion

		#region ISitesSource Members

		[EditableText(Title = "Site host name (DNS)", ContainerName = Defaults.Containers.Site)]
		public virtual string HostName { get; set; }

		public IEnumerable<Site> GetSites()
		{
			if (!string.IsNullOrEmpty(HostName))
				yield return new Site(Find.EnumerateParents(this, null, true).Last().ID, ID, HostName) { Wildcards = true };
		}

		#endregion
	}
}