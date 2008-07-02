using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using N2.Templates.Items;
using N2.Integrity;
using N2.Details;

namespace N2.Templates.Wiki.Items
{
    [Definition(SortOrder = 460)]
    [RestrictParents(typeof(IStructuralPage))]
    public class Wiki : WikiArticle
    {
        public Wiki()
        {
            Visible = true;
        }

        [WikiText("Sidebar Text", 110, ContainerName = Tabs.Content)]
        public virtual string SidebarText
        {
            get { return (string)(GetDetail("SidebarText") ?? string.Empty); }
            set { SetDetail("SidebarText", value, string.Empty); }
        }

        public string NewArticleName { get; set; }

        public override ContentItem  WikiRoot
        {
            get { return this; }
        }

        public override ContentItem GetChild(string childName)
        {
            ContentItem article = base.GetChild(childName) ?? base.GetChild(childName.Replace(' ', '-'));
            if (article == null)
            {
                NewArticleName = childName;
                return this;
            }
            return article;
        }

        public override string TemplateUrl
        {
            get
            {
                if (NewArticleName != null)
                    return "~/Wiki/UI/SubmitArticle.aspx";
                return base.TemplateUrl;
            }
        }
    }
}
