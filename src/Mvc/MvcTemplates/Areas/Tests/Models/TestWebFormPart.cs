#if DEBUG
using N2.Details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Templates.Mvc.Areas.Tests.Models
{
    [PartDefinition("Test WebForm", TemplateUrl = "~/Areas/Tests/Views/Forms/TestWebFormPart.ascx", SortOrder = 21002)]
    public class TestWebFormPart : ContentItem, IWebFormPart
    {
        [EditableText]
        public virtual string Text { get; set; }
    }
}
#endif
