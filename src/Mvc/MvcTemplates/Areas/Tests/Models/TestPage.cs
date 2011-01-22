#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Details;
using N2.Integrity;
using N2.Templates.Mvc.Models.Pages;

namespace N2.Templates.Mvc.Areas.Tests.Models
{
	[PageDefinition(SortOrder = int.MaxValue, Description = "Release compile the project to remove this test")]
	[RestrictParents(typeof(StartPage))]
	[WithEditableTitle, WithEditableName]
	public class TestPage : TestItemBase
	{
	}
}
#endif
