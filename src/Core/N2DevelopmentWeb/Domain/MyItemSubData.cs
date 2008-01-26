using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace N2.TemplateWeb.Domain
{
    [N2.Definition("My sub data", "MyItemSubData"),
        N2.Integrity.AllowedZones("Main", "Content"),
	   N2.Integrity.RestrictParents(typeof(MyPageData), typeof(MyItemData), typeof(MyItemSubData)),
		N2.Details.WithEditable("Zone", typeof(N2.Web.UI.WebControls.ZoneSelector), "SelectedValue", 50, "ZoneName")]
    public class MyItemSubData : N2.ContentItem
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

        public override bool IsPage
        {
            get
            {
                return false;
            }
        }

        public override string TemplateUrl
        {
            get { return "~/Uc/MySpecialItem.ascx"; }
        }
    }
}
