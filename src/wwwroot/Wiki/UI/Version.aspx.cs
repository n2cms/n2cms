using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;

namespace N2.Templates.Wiki.UI
{
    public partial class Version : WikiTemplatePage
    {
        protected Items.WikiArticle CurrentVersion
        {
            get
            {
                int versionID = int.Parse(CurrentPage.ActionParameter);
                return Engine.Persister.Get<N2.Templates.Wiki.Items.WikiArticle>(versionID);
            }
        }
    }
}
