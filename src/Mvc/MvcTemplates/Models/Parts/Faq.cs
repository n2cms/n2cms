using System;
using N2.Integrity;
using N2.Details;
using N2.Templates.Mvc.Models.Pages;
using N2.Web.UI.WebControls;

namespace N2.Templates.Mvc.Models.Parts
{
    [PartDefinition("FAQ Item",
        Description = "A question with answer.",
        SortOrder = 0,
        IconUrl = "~/Content/Img/information.png")]
    [RestrictParents(typeof (FaqList))]
    [AllowedZones("Questions")]
    [WithEditableTitle("Question", 90, Focus = false)]
    public class Faq : PartBase
    {
        [DisplayableHeading(3)]
        public override string Title
        {
            get { return base.Title; }
            set { base.Title = value; }
        }

        [EditableFreeTextArea("Answer", 100)]
        public virtual string Answer
        {
            get { return (string) (GetDetail("Answer") ?? string.Empty); }
            set { SetDetail("Answer", value, string.Empty); }
        }
    }
}
