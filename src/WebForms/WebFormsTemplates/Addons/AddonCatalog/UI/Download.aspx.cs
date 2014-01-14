using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Resources;
using System.Web.UI.HtmlControls;
using N2.Templates.Web.UI;

namespace N2.Addons.AddonCatalog.UI
{
    public partial class Download : TemplatePage<Items.Addon>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if(string.IsNullOrEmpty(CurrentPage.UploadedFileUrl))
                Response.Redirect(CurrentPage.Url);

            HtmlGenericControl meta = new HtmlGenericControl("meta");
            meta.Attributes["http-equiv"] = "refresh";
            meta.Attributes["content"] = "1;URL=" + CurrentPage.UploadedFileUrl;

            Page.Header.Controls.Add(meta);

            CurrentPage.Downloads++;
            Engine.Persister.Save(CurrentPage);
        }
    }
}
