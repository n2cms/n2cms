#if DEBUG
using N2.Details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Templates.Mvc.Areas.Tests.Models
{
	[PartDefinition(SortOrder = 21001)]
	public class TestPart : TestItemBase
	{
        [EditableItem("EditableChild", 100)]
        public virtual TestWebFormPart EditableItem { get; set; }

        [EditableChildren("Children", "Children", 200)]
        public virtual IEnumerable<TestWebFormPart> EditableChildren { get; set; }
	}
}
#endif