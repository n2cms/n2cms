using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2;
using N2.Integrity;
using N2.Definitions;

namespace Dinamico.Models
{
	[PageDefinition]
	[RestrictParents(typeof(IRootPage))]
	public class StartPage : TextPage , IStartPage, IStructuralPage
	{
	}
}