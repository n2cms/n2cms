using System;
using N2.Web;
using log4net;

namespace N2.Templates.UI.Views
{
    public partial class Error500 : System.Web.UI.Page
    {
    	private readonly ILog logger = LogManager.GetLogger(typeof (Error500));

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
					Server.Execute(Url.Parse(page.FindPath(PathData.DefaultAction).RewrittenUrl).AppendQuery("postback", page.Url));
                    Response.End();
                    return;
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex.ToString());
            }
            Response.Write("<html><body><h1>500 Internal Server Error</h1></body></html>");
            Response.End();
        }
    }
}