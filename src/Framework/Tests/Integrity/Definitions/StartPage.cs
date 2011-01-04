using System;
using System.Collections.Generic;
using System.Text;

using N2;
using N2.Integrity;

namespace N2.Tests.Integrity.Definitions
{
	[PageDefinition]
	[RestrictParents(AllowedTypes.None)] // no parents allowed
	public class StartPage : N2.ContentItem
	{
	}
}
