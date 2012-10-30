#if DEBUG
using N2.Details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Templates.Mvc.Areas.Tests.Models
{
    [PartDefinition(Title = "TestPart [child]", SortOrder = 21002)]
    public class TestPartWithEditableChild : TestPart
    {
        [EditableItem("EditableItem", 100)]
        public virtual TestWebFormPart EditableItem { get; set; }
    }

    [PartDefinition(Title = "TestPart [children]", SortOrder = 21003)]
    public class TestPartWithEditableChildren : TestPart
    {
        [EditableChildren("Children", "EditableChildren", 200)]
        public virtual IEnumerable<TestWebFormPart> EditableChildren { get; set; }
    }

	[PartDefinition(SortOrder = 21001)]
	public class TestPart : TestItemBase
	{
	}
}
#endif