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

namespace N2.Templates.Wiki.Items
{
    [Definition]
    [RestrictParents(typeof(Wiki))]
    public class WikiArticle : AbstractContentPage, IArticle
    {
        public WikiArticle()
        {
            Visible = false;
        }

        public string Action { get; set; }

        [WikiText("Wiki", 100, ContainerName = Tabs.Content)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        public override string TemplateUrl
        {
            get
            {
                if (string.IsNullOrEmpty(Action))
                    return base.TemplateUrl;
                else
                    throw new NotImplementedException();
            }
        }

        public virtual ContentItem WikiRoot
        {
            get { return this.Parent; }
        }
    }
}
