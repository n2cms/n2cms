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
	[PageDefinition]
	[RestrictParents(typeof(IRootPage))]
	public class StartPage : TextPage , IStartPage, IStructuralPage, IThemeable
	{
		#region IThemeable Members

		[EditableThemeAttribute]
		public virtual string Theme { get; set; }

		#endregion
	}
}