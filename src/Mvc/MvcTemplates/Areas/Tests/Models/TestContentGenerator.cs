#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Details;
using N2.Definitions;

namespace N2.Templates.Mvc.Areas.Tests.Models
{
    [PartDefinition("Test Content Creator", SortOrder = 21000)]
    [Versionable(AllowVersions.No)]
    [Throwable(AllowInTrash.No)]
    public class TestContentGenerator : TestItemBase
    {
        public TestContentGenerator()
        {
        }
        [EditableCheckBox]
        public virtual bool ShowEveryone { get; set; }
    }
}
#endif
