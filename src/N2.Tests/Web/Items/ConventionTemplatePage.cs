using N2.Web;

[assembly: ConventionTemplateDirectory("~/Tests/UI/Views/")]

namespace N2.Tests.Web.Items
{
	[Definition, ConventionTemplate]
	public class ConventionTemplatePage : ContentItem
	{
	}

	[Definition, ConventionTemplate("SomeOtherTemplate")]
	public class ConventionTemplatePage2 : ContentItem
	{
	}

}
