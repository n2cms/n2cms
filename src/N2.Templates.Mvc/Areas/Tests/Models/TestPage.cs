#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Details;

namespace N2.Templates.Mvc.Areas.Tests.Models
{
	[PageDefinition(SortOrder = int.MaxValue, Description = "Release compile the project to remove this test")]
	[WithEditableTitle]
	public class TestPage : TestItemBase
	{
	}
}
#endif
