using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using N2.Integrity;
using N2.Details;

namespace N2.Templates.Items
{
    [PartDefinition("Bubble",
        IconUrl = "~/Templates/UI/Img/help.png")]
    [AllowedZones(Zones.Left, Zones.Right, Zones.ColumnLeft, Zones.ColumnRight)]
    public class BubbleItem : AbstractItem
    {
        [EditableFreeTextArea("Text", 100)]
        public virtual string Text
        {
            get { return (string)(GetDetail("Text") ?? string.Empty); }
            set { SetDetail("Text", value, string.Empty); }
        }

        protected override string TemplateName
        {
            get { return "Bubble"; }
        }
    }
}
