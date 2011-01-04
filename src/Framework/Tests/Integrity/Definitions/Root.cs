using System;
using System.Collections.Generic;
using System.Text;
using N2.Integrity;

namespace N2.Tests.Integrity.Definitions
{
	[PageDefinition]
	[AllowedChildren(typeof(StartPage))]
	public class Root : N2.ContentItem
	{
	}
}
