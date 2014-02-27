using N2.Web;

[assembly: ConventionTemplateDirectory("~/Tests/UI/Views/")]

namespace N2.Tests.Web.Items
{
    [PageDefinition, ConventionTemplate]
    public class ConventionTemplatePage : ContentItem
    {
    }

    [PageDefinition, ConventionTemplate("SomeOtherTemplate")]
    public class ConventionTemplatePage2 : ContentItem
    {
    }

}
