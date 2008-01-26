using System;
using System.Collections.Generic;
using System.Text;

using N2;

namespace N2.Tests.Integrity.Definitions
{
	[N2.Integrity.RestrictParents(N2.Integrity.AllowedTypes.None)] // no parents allowed
	public class StartPage : N2.ContentItem
	{
	}
}
