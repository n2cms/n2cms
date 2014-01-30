using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using N2.Integrity;
using N2.Details;
using N2.Templates.Items;
using N2.Web.UI.WebControls;

namespace N2.Templates.Items
{
    [PartDefinition("FAQ Item", 
        Description = "A question with answer.",
        SortOrder = 0,
        IconUrl = "~/Templates/UI/Img/information.png")]
    [RestrictParents(typeof(FaqList))]
    [AllowedZones("Questions")]
    [WithEditableTitle("Question", 90, Focus = false)]
    public class Faq : AbstractItem
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
            get { return (string)(GetDetail("Answer") ?? string.Empty); }
            set { SetDetail("Answer", value, string.Empty); }
        }

        protected override string TemplateName
        {
            get { return "Faq"; }
        }
    }
}
