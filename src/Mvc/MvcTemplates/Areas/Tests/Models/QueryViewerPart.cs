#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Templates.Mvc.Areas.Tests.Models
{
	[PartDefinition("Query viewer", TemplateUrl = "~/Addons/UITests/UI/QueryViewer.ascx", SortOrder = 20000)]
	public class QueryViewerPart : TestItemBase
	{
	}
}
#endif