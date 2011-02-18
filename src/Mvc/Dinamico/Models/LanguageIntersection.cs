using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2;
using N2.Integrity;
using N2.Definitions;
using N2.Details;

namespace Dinamico.Models
{
	[PageDefinition(
		IconUrl = "{IconsUrl}/world_go.png",
		InstallerVisibility = N2.Installation.InstallerHint.PreferredStartPage)]
	[RestrictParents(typeof(IRootPage))]
	public class LanguageIntersection : TextPage, IThemeable
	{
		#region IThemeable Members

		[EditableThemeAttribute]
		public virtual string Theme { get; set; }

		#endregion
	}
}