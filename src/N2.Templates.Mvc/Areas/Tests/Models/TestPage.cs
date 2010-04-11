#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Details;

namespace N2.Templates.Mvc.Areas.Tests.Models
{
	[PageDefinition(SortOrder = int.MaxValue)]
	[WithEditableTitle]
	public class TestPage : TestItemBase
	{
	}
}
#endif
