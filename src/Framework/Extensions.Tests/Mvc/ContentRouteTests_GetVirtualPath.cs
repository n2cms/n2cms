using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using NUnit.Framework;

namespace N2.Extensions.Tests.Mvc
{
    [TestFixture]
    public class ContentRouteTests_GetVirtualPath : ContentRouteTests
    {
        [Test]
        public void VirtualPath_IsNull_ForOtherRequestedController()
        {
            RequestingUrl("/about/");

            var virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { controller = "Mvc" }));

            Assert.That(virtualPath, Is.Null);
        }

        [Test]
        public void AppendsActionName_ToContentUrl()
        {
            RequestingUrl("/about/");

            var result = urlHelper.Action("hello");

            Assert.That(result, Is.EqualTo("/about/hello"));
        }

        [Test]
        public void DoesntRoute_ControllerOnly()
        {
            RequestingUrl("/about/");

            var result = urlHelper.Action("hello", "other");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void CanCreate_ActionUrl_ToPage()
        {
            RequestingUrl("/about/");

            var result = urlHelper.Action("Index", new { n2item = executives });

            Assert.That(result, Is.EqualTo("/about/executives"));
        }

        [Test]
        public void GetVirtualPath_ToSelf()
        {
            RequestingUrl("/search/");

            var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary());

            Assert.That(vpd.VirtualPath, Is.EqualTo("search"));
        }

        [Test]
        public void GetVirtualPath_ToOtherAction_OnSelf()
        {
            RequestingUrl("/search/");

            var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { action = "find" }));

            Assert.That(vpd.VirtualPath, Is.EqualTo("search/find"));
        }

        [Test]
        public void GetVirtualPath_ToOtherAction_OnSelf_WithParameter()
        {
            RequestingUrl("/search/");

            var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { action = "find", q = "query" }));

            Assert.That(vpd.VirtualPath, Is.EqualTo("search/find?q=query"));
        }

        [Test]
        public void GetVirtualPathTo_OtherContentItem()
        {
            RequestingUrl("/search/");

            var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { n2item = executives }));

            Assert.That(vpd.VirtualPath, Is.EqualTo("about/executives"));
        }

        [Test]
        public void GetVirtualPathTo_OtherContentItem_ViaPage_IsNoLongerSupported()
        {
            RequestingUrl("/search/");

            var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { n2page = executives }));

            Assert.That(vpd.VirtualPath, Is.Not.EqualTo("about/executives"));
        }

        [Test]
        public void GetVirtualPathTo_SameController_IsNotNull()
        {
            RequestingUrl("/about/");

            var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { controller = "aboutussectionpage" }));

            Assert.That(vpd, Is.Not.Null);
            Assert.That(vpd.VirtualPath, Is.EqualTo("about"));
        }

        [Test]
        public void GetVirtualPathTo_SameController_AddsAction()
        {
            RequestingUrl("/about/");

            var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { controller = "aboutussectionpage", action = "more" }));

            Assert.That(vpd, Is.Not.Null);
            Assert.That(vpd.VirtualPath, Is.EqualTo("about/more"));
        }

        [Test]
        public void GetVirtualPathTo_OtherController_IsNull()
        {
            RequestingUrl("/about/");

            var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { controller = "hello", action = "Index" }));

            Assert.That(vpd, Is.Null);
        }

        [Test]
        public void GetVirtualPath_ToAction_OnOtherContentItem()
        {
            RequestingUrl("/about/executives/");

            var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { n2item = search, action = "find" }));

            Assert.That(vpd.VirtualPath, Is.EqualTo("search/find"));
        }

        [Test]
        public void GetVirtualPath_ToAction_OnOtherContentItem_WithParameters()
        {
            RequestingUrl("/about/executives/");

            var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { n2item = search, action = "find", q = "what", x = "y" }));

            Assert.That(vpd.VirtualPath, Is.EqualTo("search/find?q=what&x=y"));
        }

        [Test]
        public void CanGenerate_DefaultRouteUrl()
        {
            RequestingUrl("/search/");

            string url = urlHelper.RouteUrl(null, new { controller = "Search" }, null);

            Assert.That(url, Is.EqualTo("/search"));
        }

        [Test]
        public void CanGenerateLink()
        {
            RequestingUrl("/search/");

            string html = HtmlHelper.GenerateLink(requestContext, routes, "Hello", null, "find", "Search", new RouteValueDictionary(new { q = "hello" }), null);

            Assert.That(html, Is.EqualTo("<a href=\"/search/find?q=hello\">Hello</a>"));
        }

        [Test]
        public void CanCreate_ActionLink()
        {
            RequestingUrl("/search/");

            var html = htmlHelper.ActionLink("Hello", "find", new { q = "something", controller = "Search" });

            Assert.That(html.ToString(), Is.EqualTo("<a href=\"/search/find?q=something\">Hello</a>"));
        }

        //[Test]
        //public void CanCreate_UrlFromExpression()
        //{
        //    RequestingUrl("/search/");

        //    string html = htmlHelper.BuildUrlFromExpression<SearchController>(s => s.Find("hello"));

        //    Assert.That(html, Is.EqualTo("/search/Find?q=hello"));
        //}

        [Test]
        public void GetVirtualPath_WhenAppInVirtualDirectory()
        {
            N2.Web.Url.ApplicationPath = "/N2Mvc/";

            RequestingUrl("~/about/");

            var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { controller = "aboutussectionpage" }));

            Assert.That(vpd, Is.Not.Null);
            Assert.That(vpd.VirtualPath, Is.EqualTo("about"));
        }
    }
}
