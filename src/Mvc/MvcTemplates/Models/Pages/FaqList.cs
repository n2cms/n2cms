using N2.Integrity;
using System.Collections.Generic;
using N2.Templates.Mvc.Models.Parts;
using N2.Web.Mvc;
using N2.Definitions;

namespace N2.Templates.Mvc.Models.Pages
{
    [PageDefinition("FAQ",
        Description = "A list of frequently asked questions with answers.",
        SortOrder = 200,
        IconClass = "fa fa-question-circle")]
    [AvailableZone("Questions", "Questions")]
    [RestrictParents(typeof (IStructuralPage))]
    public class FaqList : ContentPageBase, IStructuralPage
    {
        [N2.Details.EditableChildren("Questions", "Questions", 110, ContainerName = Tabs.Content)]
        public virtual IList<Faq> Questions
        {
            get { return GetChildren<Faq>("Questions"); }
        }
    }
}
