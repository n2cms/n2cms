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
    [Definition("FAQ Item", "Faq", "A question with answer.", "", 0)]
    [RestrictParents(typeof(FaqList))]
    [AllowedZones("Questions")]
    [WithEditableTitle("Question", 90)]
    public class Faq : Templates.Items.AbstractItem
    {
        [Displayable(typeof(H3), "Text")]
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

        protected override string IconName
        {
            get { return "information"; }
        }

        protected override string TemplateName
        {
            get { return "Faq"; }
        }
    }
}