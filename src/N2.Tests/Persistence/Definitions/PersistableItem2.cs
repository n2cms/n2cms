using System;
using System.Collections.Generic;
using System.Text;
using N2.Definitions;

namespace N2.Tests.Persistence.Definitions
{
	[SortChildren(SortBy.Expression, SortExpression = "Name DESC")]
	public class PersistableItem2 : PersistableItem1
	{
	}
}
