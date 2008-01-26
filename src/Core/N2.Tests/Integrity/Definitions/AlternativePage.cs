using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Tests.Integrity.Definitions
{
	[N2.Integrity.RestrictParents(typeof(AlternativeStartPage))] // SubClassOfRoot as parent allowed
	public class AlternativePage : ContentItem
	{
	}
}
