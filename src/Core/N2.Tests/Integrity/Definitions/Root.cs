using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Tests.Integrity.Definitions
{
	[N2.Integrity.AllowedChildren(typeof(StartPage))]
	public class Root : N2.ContentItem
	{
	}
}
