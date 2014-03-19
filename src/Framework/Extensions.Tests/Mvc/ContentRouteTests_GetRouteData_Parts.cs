using System.Web.Routing;
using N2.Extensions.Tests.Mvc.Models;
using N2.Web.Mvc;
using NUnit.Framework;

namespace N2.Extensions.Tests.Mvc
{
    [TestFixture]
    public class ContentRouteTests_GetRouteData_Parts : ContentRouteTests
    {
        [Test]
        public void CanRoute_ToPart()
        {
            var part = CreateOneItem<TestItem>(10, "whatever", root);
            RequestingUrl("/TestItem/?n2part=10");

            var r = routes.GetRouteData(httpContext);

            Assert.That(r.CurrentItem(), Is.EqualTo(part));
            Assert.That(r.CurrentPage(), Is.EqualTo(root));
            Assert.That(r.Values[ContentRoute.ControllerKey], Is.EqualTo("TestItem"));
            Assert.That(r.Values[ContentRoute.ActionKey], Is.EqualTo("Index"));
        }

        [Test]
        public void CanRoute_ToPart_WithPage()
        {
            var part = CreateOneItem<TestItem>(10, "whatever", root);
            RequestingUrl("/TestItem/?n2page=1&n2part=10");

            var r = routes.GetRouteData(httpContext);

            Assert.That(r.CurrentItem(), Is.EqualTo(part));
            Assert.That(r.CurrentPage(), Is.EqualTo(root));
            Assert.That(r.Values[ContentRoute.ControllerKey], Is.EqualTo("TestItem"));
            Assert.That(r.Values[ContentRoute.ActionKey], Is.EqualTo("Index"));
        }

        [Test]
        public void CanRoute_ToPart_Action()
        {
            var part = CreateOneItem<TestItem>(10, "whatever", root);
            RequestingUrl("/TestItem/Submit/?n2part=10");

            var r = routes.GetRouteData(httpContext);

            Assert.That(r.CurrentItem(), Is.EqualTo(part));
            Assert.That(r.Values[ContentRoute.ControllerKey], Is.EqualTo("TestItem"));
            Assert.That(r.Values[ContentRoute.ActionKey], Is.EqualTo("Submit"));
        }

        [Test]
        public void CanRoute_ToPart_Action_WithPage()
        {
            var part = CreateOneItem<TestItem>(10, "whatever", root);
            RequestingUrl("/TestItem/Submit/?n2page=1&n2part=10");

            var r = routes.GetRouteData(httpContext);

            Assert.That(r.CurrentItem(), Is.EqualTo(part));
            Assert.That(r.CurrentPage(), Is.EqualTo(root));
            Assert.That(r.Values[ContentRoute.ControllerKey], Is.EqualTo("TestItem"));
            Assert.That(r.Values[ContentRoute.ActionKey], Is.EqualTo("Submit"));
        }

        [Test]
        public void CanRoute_ToPart_Action_WithPageOtherThanParent()
        {
            var part = CreateOneItem<TestItem>(10, "whatever", root);
            RequestingUrl("/TestItem/Submit/?n2part=10&n2page=" + about.ID);

            var r = routes.GetRouteData(httpContext);

            Assert.That(r.CurrentItem(), Is.EqualTo(part));
            Assert.That(r.CurrentPage(), Is.EqualTo(about));
            Assert.That(r.Values[ContentRoute.ControllerKey], Is.EqualTo("TestItem"));
            Assert.That(r.Values[ContentRoute.ActionKey], Is.EqualTo("Submit"));
        }

        [Test]
        public void CanRoute_ToPart_PageIsClosestParent()
        {
            var part = CreateOneItem<TestItem>(10, "whatever", root);
            RequestingUrl("/TestItem/Submit/?n2part=10");

            var r = routes.GetRouteData(httpContext);

            Assert.That(r.CurrentItem(), Is.EqualTo(part));
            Assert.That(r.CurrentPage(), Is.EqualTo(root));
            Assert.That(r.Values[ContentRoute.ControllerKey], Is.EqualTo("TestItem"));
            Assert.That(r.Values[ContentRoute.ActionKey], Is.EqualTo("Submit"));
        }

        [Test]
        public void RoutingToPart_ViaItsVirtualPath_PassesController_OfThePart()
        {
            RequestingUrl("/");

            var part = CreateOneItem<TestItem>(10, "whatever", root);
            var vpd = routes.GetVirtualPath(requestContext, new RouteValueDictionary(new { n2item = part }));
            RequestingUrl(vpd.VirtualPath);

            var r = routes.GetRouteData(httpContext);
            Assert.That(r.Values[ContentRoute.ControllerKey], Is.EqualTo("TestItem"));
        }

        [Test]
        public void RoutingToPart_ViaItsVirtualPath_PassesAssociatedPart_AsContentPart()
        {
            var part = CreateOneItem<TestItem>(10, "whatever", root);

            RequestingUrl("/");

            var vpd = routes.GetVirtualPath(requestContext, new RouteValueDictionary(new { n2item = part }));
            RequestingUrl(vpd.VirtualPath);
            var r = routes.GetRouteData(httpContext);

            Assert.That(r.CurrentItem(), Is.EqualTo(part));
            Assert.That(r.CurrentPage(), Is.EqualTo(root));
        }

        [Test]
        public void RoutingToPart_PassesPart_AsContentPart()
        {
            var part = CreateOneItem<TestItem>(10, "whatever", root);
            var r = RequestingUrl("/TestItem/?n2part=10");

            Assert.That(r.CurrentPath().ID, Is.EqualTo(part.ID));
            Assert.That(r.CurrentPath().CurrentItem, Is.EqualTo(part));
        }

        [Test]
        public void RoutingToPart_PassesAssociatedPage_AsContentPage()
        {
            var part = CreateOneItem<TestItem>(10, "whatever", root);
            var r = RequestingUrl("/TestItem/?n2part=10");

            Assert.That(r.CurrentPath().PageID, Is.EqualTo(root.ID));
            Assert.That(r.CurrentPath().CurrentPage, Is.EqualTo(root));
        }

        [Test]
        public void RoutingToPart_PassesController_OfThePart()
        {
            var part = CreateOneItem<TestItem>(10, "whatever", root);
            var r = RequestingUrl("/TestItem/?n2part=10");

            Assert.That(r.Values[ContentRoute.ControllerKey], Is.EqualTo("TestItem"));
        }

        [Test]
        public void RoutingToPart_PassesAction_DefinedByQueryString()
        {
            var part = CreateOneItem<TestItem>(10, "whatever", root);
            RequestingUrl("/TestItem/WithModel/?n2part=10");

            var r = routes.GetRouteData(httpContext);
            Assert.That(r.Values[ContentRoute.ActionKey], Is.EqualTo("WithModel"));
        }

        [Test]
        public void RoutingToPart_PassesPage_AsContentPage()
        {
            var part = CreateOneItem<TestItem>(10, "whatever", root);
            var r = RequestingUrl("/TestItem/?n2part=10&n2page=" + search.ID);

            Assert.That(r.CurrentPath().PageID, Is.EqualTo(search.ID));
            Assert.That(r.CurrentPath().CurrentPage, Is.EqualTo(search));
        }

        [Test]
        public void RoutingToPart_PassesAssociatedPart_AsContentItem()
        {
            RequestingUrl("/");

            var part = CreateOneItem<TestItem>(10, "whatever", root);
            var vpd = routes.GetVirtualPath(requestContext, new RouteValueDictionary(new { n2item = part }));
            RequestingUrl(vpd.VirtualPath);

            var r = routes.GetRouteData(httpContext);
            Assert.That(r.CurrentPath().ID, Is.EqualTo(part.ID));
            Assert.That(r.CurrentPath().CurrentItem, Is.EqualTo(part));
        }

        [Test, Ignore("TODO (maybe)")]
        public void CanRoute_ToPartVirtualPath_Passes_AssociatedPage()
        {
            RequestingUrl("/");

            var part = CreateOneItem<TestItem>(10, "whatever", root);
            var vpd = routes.GetVirtualPath(requestContext, new RouteValueDictionary(new { n2item = part }));
            RequestingUrl(vpd.VirtualPath);

            var r = routes.GetRouteData(httpContext);
            Assert.That(r.Values[ContentRoute.ContentPageKey], Is.EqualTo(root.ID.ToString()));
            Assert.That(r.DataTokens[ContentRoute.ContentPageKey], Is.EqualTo(root));
        }
    }

}
