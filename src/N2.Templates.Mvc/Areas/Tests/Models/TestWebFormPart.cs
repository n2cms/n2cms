#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Templates.Mvc.Areas.Tests.Models
{
	[PartDefinition(TemplateUrl = "~/Areas/Tests/Views/Forms/TestWebFormPart.ascx")]
	public class TestWebFormPart : ContentItem
	{
	}
}
#endif