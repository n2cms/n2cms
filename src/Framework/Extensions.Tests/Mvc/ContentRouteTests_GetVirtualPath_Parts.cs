using System.Web.Mvc.Html;
using System.Web.Routing;
using N2.Extensions.Tests.Mvc.Models;
using N2.Web.Mvc;
using NUnit.Framework;

namespace N2.Extensions.Tests.Mvc
{
    [TestFixture]
    public class ContentRouteTests_GetVirtualPath_Parts : ContentRouteTests
    {
        [Test]
        public void GetVirtualPath_ToPartDefaultAction()
        {
            var part = CreateOneItem<TestItem>(10, "whatever", about);
            RequestingUrl("/about/");

            var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { n2item = part }));

            Assert.That(vpd.VirtualPath, Is.EqualTo("TestItem?n2page=2&n2part=10"));
        }

        [Test]
        public void GetVirtualPath_ToPartDefaultAction_ViaPartQuery_IsNotSupported()
        {
            var part = CreateOneItem<TestItem>(10, "whatever", about);
            RequestingUrl("/about/");

            var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { n2part = part }));

            var data = RequestingUrl(vpd.VirtualPath);
            Assert.That(data.CurrentItem(), Is.EqualTo(about));
        }

        [Test]
        public void GetVirtualPath_ToPart_OtherActionAction()
        {
            var part = CreateOneItem<TestItem>(10, "whatever", about);
            RequestingUrl("/about/");

            var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { n2item = part, action = "doit" }));

            Assert.That(vpd.VirtualPath, Is.EqualTo("TestItem/doit?n2page=2&n2part=10"));
        }

        [Test]
        public void CanCreate_ActionLink_ToPart()
        {
            var item = CreateOneItem<TestItem>(10, "whatever", root);
            RequestingUrl("/?n2part=10");

            var html = htmlHelper.ActionLink(/*text*/"Hello", /*action*/"Submit", new { q = "something", controller = "TestItem" });

            Assert.That(html.ToString(), Is.EqualTo("<a href=\"/TestItem/Submit?q=something&amp;n2page=1&amp;n2part=10\">Hello</a>"));
        }
    }
}
