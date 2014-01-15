using System;
using System.Web.UI;
using N2.Web;

namespace N2.Templates.UI.Views
{
    public class NotFound404 : Page
    {
        protected override void OnInit(EventArgs args)
        {
            try
            {
                N2.ContentItem page = N2.Templates.Find.StartPage.NotFoundPage;
                if (page != null)
                {
                    var wc = N2.Context.Current.Resolve<N2.Web.IWebContext>();
                    wc.CurrentPage = page;
                    Server.Execute(Url.Parse(page.FindPath(PathData.DefaultAction).GetRewrittenUrl()).AppendQuery("postback", page.Url));
                    Response.Status = "404 Not Found";
                    Response.End();
                    return;
                }
            }
            catch
            {
            }
            Response.Status = "404 Not Found";
            Response.Write("<html><body><h1>404 Not Found</h1></body></html>");
            Response.End();
        }
    }
}
