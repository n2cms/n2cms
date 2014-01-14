using System;
using System.Web.Mvc;
using N2.Web;
using N2.Templates.Mvc.Models.Pages;

namespace N2.Templates.Mvc.Controllers
{
    [Controls(typeof(Redirect))]
    public class RedirectController : TemplatesControllerBase<Redirect>
    {
        public override System.Web.Mvc.ActionResult Index()
        {
            var redirectUrl = N2.Web.Url.ToAbsolute(CurrentItem.RedirectUrl);

            if (CurrentItem.Redirect301)
            {
                return new PermanentRedirectResult(redirectUrl);
            }

            return Redirect(redirectUrl);
        }

        public class PermanentRedirectResult : ActionResult
        {
            public string Url { get; set; }

            public PermanentRedirectResult(string url)
            {
                if (string.IsNullOrEmpty(url))
                {
                    throw new ArgumentException(Resources.Redirect.UrlNull, "url");
                }
                Url = url;
            }

            public override void ExecuteResult(ControllerContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }
                context.HttpContext.Response.StatusCode = 301;
                context.HttpContext.Response.RedirectLocation = Url;
                context.HttpContext.Response.End();
            }
        }
    }
}
