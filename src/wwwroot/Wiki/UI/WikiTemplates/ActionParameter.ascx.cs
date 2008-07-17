using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using N2.Templates.Wiki.Web;

namespace N2.Templates.Wiki.UI.WikiTemplates
{
    [WikiTemplate("~/Wiki/UI/WikiTemplates/ActionParameter.ascx")]
    public partial class ActionParameter : WikiUserControl<Items.WikiArticle>
    {
    }
}