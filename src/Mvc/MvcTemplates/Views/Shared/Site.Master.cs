using System;
using N2.Web.Mvc;
using N2.Web.Mvc.Html;
using N2.Web.UI;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web;
using N2.Templates.Mvc.Services;
using N2.Templates.Mvc.Models.Pages;
using N2.Web;

namespace N2.Templates.Mvc.Views.Shared
{
    public partial class Site : N2.Web.UI.MasterPage<ContentItem>
    {
        protected StartPage Start { get; set; }
        protected LanguageRoot Language { get; set; }
        protected HtmlHelper Html { get; set; }

        protected override void OnInit(EventArgs e)
        {
            ViewPage view = Page as ViewPage;
            if (view != null)
                Html = view.Html;
            else
                Html = BuildHtmlHelper();

            var item = Html.CurrentPage();
            if (item != null)
            {
                foreach (var vc in Engine.Container.ResolveAll<IViewConcern>())
                    vc.Apply(item, Page);
            }
            Start = Find.Closest<StartPage>(item) ?? Find.ClosestStartPage ?? Find.StartPage;
            Language = Find.Closest<LanguageRoot>(item) ?? Find.ClosestLanguageRoot ?? Find.StartPage;
            base.OnInit(e);
        }

        private HtmlHelper BuildHtmlHelper()
        {
            var ctx = new HttpContextWrapper(Context);
            var rd = new RouteData();

            var currentPage = (Page is IItemContainer) ? (Page as IItemContainer).CurrentItem : N2.Context.CurrentPage;
            RouteExtensions.ApplyCurrentPath(rd, "WebForms", "Index", new PathData(currentPage, currentPage));
            rd.DataTokens[ContentRoute.ContentEngineKey] = N2.Context.Current;

            var rqctx = new RequestContext(ctx, rd);
            var ctrlctx = new ControllerContext { HttpContext = ctx, RequestContext = rqctx, RouteData = rqctx.RouteData };
            var wfv = new WebFormView(ctrlctx, Request.CurrentExecutionFilePath);

            var vctx = new ViewContext(ctrlctx, wfv, new ViewDataDictionary(), new TempDataDictionary(), Response.Output) { RouteData = rd };

            return new HtmlHelper<ContentItem>(vctx, new ViewPage());
        }

        protected string GetBodyClass()
        {
            if (CurrentItem != null)
            {
                string className = CurrentItem.GetContentType().Name;
                return className.Substring(0, 1).ToLower() + className.Substring(1);
            }
            return null;
        }
    }
}
