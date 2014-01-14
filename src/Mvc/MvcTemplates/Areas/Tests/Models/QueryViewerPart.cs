#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Definitions;

namespace N2.Templates.Mvc.Areas.Tests.Models
{
    [PartDefinition("Query viewer", TemplateUrl = "~/Addons/UITests/UI/QueryViewer.ascx", SortOrder = 20000)]
    [Versionable(AllowVersions.No)]
    [Throwable(AllowInTrash.No)]
    public class QueryViewerPart : TestItemBase
    {

        [N2.Details.EditableCheckBox("Visible to everyone", 100)]
        public virtual bool VisibleToEveryone
        {
            get { return GetDetail("VisibleToEveryone", false); }
            set { SetDetail("VisibleToEveryone", value, false); }
        }
        
    }
}
#endif
