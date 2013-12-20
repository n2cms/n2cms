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

    [PartDefinition(Title = "TestPart [link]", SortOrder = 21002)]
    public class TestPartWithEditableLink : TestPart
    {
        [EditableLink]
        public virtual ContentItem EditableLink { get; set; }
    }

    [PartDefinition(Title = "TestPart [children]", SortOrder = 21003)]
    public class TestPartWithEditableChildren : TestPart
    {
        [EditableChildren("Children", "EditableChildren", 200)]
        public virtual IEnumerable<TestWebFormPart> EditableChildren { get; set; }
    }

    [PartDefinition(Title = "TestPart [item selection]", SortOrder = 21004)]
    public class TestPartWithEditableItemSelection : TestPart
    {
        [EditableMultipleItemSelection]
        public virtual IEnumerable<ContentItem> EditableItemSelection { get; set; }
    }

    [PartDefinition(Title = "TestPart [multiple]", SortOrder = 21005)]
    public class TestPartWithMultipleEditables : TestPart
    {
        [EditableMultipleItemSelection(LinkedType = typeof(N2.Templates.Mvc.Models.Pages.ContentPageBase))]
        public virtual IEnumerable<N2.Templates.Mvc.Models.Pages.ContentPageBase> Pages { get; set; }

        [EditableCheckBoxList]
        public virtual List<string> EditableCheckBoxList { get; set; }

        [EditableChildren(ZoneName = "EditableChildrenZone")]
        public virtual IEnumerable<TestPartWithEditableChild> EditableChildren { get; set; }

        [EditableLink]
        public virtual ContentItem EditableLink { get; set; }

        [EditableItem("EditableItem", 100)]
        public virtual TestPartWithEditableChild EditableItem { get; set; }
    }

    [PartDefinition(SortOrder = 21001)]
    public class TestPartWithText : TestPart
    {
        [EditableText]
        public string Text { get; set; }
    }

    [PartDefinition(SortOrder = 21001)]
    public class TestPart : TestItemBase
    {
    }
}
#endif
