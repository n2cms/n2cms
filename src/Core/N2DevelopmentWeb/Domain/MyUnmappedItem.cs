using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace N2DevelopmentWeb.Domain
{
    [N2.Definition("My unmapped page"),
        N2.Integrity.AvailableZone("Right", "Right"),
        N2.Integrity.AvailableZone("Content", "Content"),
        N2.Integrity.RestrictParents(typeof(MyPageData), typeof(MySpecialPageData)),
        N2.Details.WithEditable("Zone", typeof(N2.Web.UI.WebControls.ZoneSelector), "SelectedValue", 50, "ZoneName")]
    public class MyUnmappedItem : AbstractCustomItem
    {
        [N2.Details.Editable("Text", typeof(N2.Web.UI.WebControls.FreeTextArea), "Text", 110)]
        public virtual string Text
        {
            get { return (string)GetDetail("Text"); }
            set { SetDetail("Text", value); }
        }

        [N2.Details.Editable("File", typeof(N2.Web.UI.WebControls.UrlSelector), "Url", 30)]
        public virtual string FileUrl
        {
            get { return (string)GetDetail("FileUrl"); }
            set { SetDetail("FileUrl", value); }
        }

		public override string TemplateUrl
		{
			get
			{
				return "~/UnmappedItem.aspx";
			}
		}
    }
}
