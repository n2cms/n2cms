using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using N2.Web;

namespace N2.Templates.UI
{
	public class NotFound404 : Page
	{
		protected override void OnInit(EventArgs args)
		{
			Response.Status = "404 Not Found";

			try
			{
				N2.ContentItem page = N2.Templates.Find.StartPage.NotFoundPage;
				if (page != null)
				{
                    var wc = N2.Context.Current.Resolve<N2.Web.IWebContext>();
                    wc.CurrentPage = page;
                    Server.Execute(Url.Parse(page.RewrittenUrl).AppendQuery("postback", page.Url));
                    Response.End();
                    return;
                }
			}
			catch
			{
			}
			Response.Write("<html><body><h1>404 Not Found</h1></body></html>");
            Response.End();
		}
	}
}
