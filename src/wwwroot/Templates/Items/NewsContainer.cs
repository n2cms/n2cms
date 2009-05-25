using N2.Integrity;
using N2.Web;

namespace N2.Templates.Items
{
    [PageDefinition("News Container", 
		Description = "A list of news. News items can be added to this page.",
		SortOrder = 150,
		IconUrl = "~/Templates/UI/Img/newspaper_link.png")]
    [RestrictParents(typeof (IStructuralPage))]
	[ConventionTemplate("NewsList")]
    public class NewsContainer : AbstractContentPage
    {
    }
}