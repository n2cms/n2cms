using N2.Integrity;
using System.Collections.Generic;
using N2.Web;
using N2.Definitions;

namespace N2.Templates.Items
{
    [PageDefinition("FAQ", 
        Description = "A list of frequently asked questions with answers.",
        SortOrder = 200,
        IconUrl = "~/Templates/UI/Img/help.png")]
    [AvailableZone("Questions", "Questions")]
    [RestrictParents(typeof(IStructuralPage))]
    [ConventionTemplate]
    public class FaqList : AbstractContentPage, IStructuralPage
    {
        [N2.Details.EditableChildren("Questions", "Questions", 110, ContainerName=Tabs.Content)]
        public virtual IList<Faq> Questions
        {
            get { return GetChildren<Faq>("Questions"); }
        }
    }
}
