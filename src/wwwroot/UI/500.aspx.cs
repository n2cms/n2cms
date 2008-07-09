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
using N2.Web;

namespace N2.Templates.UI
{
    public partial class Error500 : System.Web.UI.Page
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
        }
        protected override void OnInit(EventArgs args)
        {
            Response.Status = "500 Internal Server Error";
            try
            {
                N2.ContentItem page = N2.Templates.Find.StartPage.ErrorPage;
                if (page != null)
                {
                    var wc = N2.Context.Current.Resolve<N2.Web.IWebContext>();
                    wc.CurrentPage = page;
                    Server.Execute(Url.Parse(page.RewrittenUrl).AppendQuery("postback", page.Url));
                    Response.End();
                    return;
                }
            }
            catch(Exception ex)
            {
                Trace.Write(ex.ToString());
            }
            Response.Write("<html><body><h1>500 Internal Server Error</h1></body></html>");
            Response.End();
        }
    }
}
