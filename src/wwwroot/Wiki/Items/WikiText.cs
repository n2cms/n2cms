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

namespace N2.Templates.Wiki.Items
{
    [Definition]
    [AllowedZones(AllowedZones.All)]
    [RestrictParents(typeof(WikiArticle))]
    public class WikiText : Templates.Items.AbstractItem, IArticle
    {
        public override string TemplateUrl
        {
            get { return "~/Wiki/UI/Parts/WikiPart.ascx"; }
        }

        #region IArticle Members

        [WikiText("Wiki Text", 100)]
        public virtual string Text
        {
            get { return (string)(GetDetail("Text") ?? string.Empty); }
            set { SetDetail("Text", value, string.Empty); }
        }

        public IWiki WikiRoot
        {
            get { return Parent as IWiki; }
        } 

        public string Action
        {
            get { return string.Empty; }
        }

        public string ActionParameter
        {
            get { return string.Empty; }
        }

        #endregion
    }
}
