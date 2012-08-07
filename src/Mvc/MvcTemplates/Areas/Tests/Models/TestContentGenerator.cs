#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Details;

namespace N2.Templates.Mvc.Areas.Tests.Models
{
	[PartDefinition("Test Content Creator", SortOrder = 21000)]
	public class TestContentGenerator : TestItemBase
	{
		[EditableCheckBox]
		public virtual bool ShowEveryone { get; set; }
	}
}
#endif