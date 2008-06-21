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
    public class WikiArticle : AbstractContentPage
    {
    }
}
