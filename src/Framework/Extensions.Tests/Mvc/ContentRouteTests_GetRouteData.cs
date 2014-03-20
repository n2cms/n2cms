using N2.Extensions.Tests.Mvc.Models;
using N2.Web.Mvc;
using NUnit.Framework;

namespace N2.Extensions.Tests.Mvc
{
    [TestFixture]
    public class ContentRouteTests_GetRouteData : ContentRouteTests
    {
        [Test]
        public void CanFindController_ForStartPage()
        {
            var data = RequestingUrl("/");

            Assert.That(data.CurrentItem(), Is.EqualTo(root));
            Assert.That(data.CurrentPage(), Is.EqualTo(root));
            Assert.That(data.Values["controller"], Is.EqualTo("Regular"));
            Assert.That(data.Values["action"], Is.EqualTo("Index"));
        }

        [Test]
        public void CanFindController_ForStartPage_default_aspx()
        {
            var data = RequestingUrl("/Default.aspx");

            Assert.That(data.CurrentItem(), Is.EqualTo(root));
            Assert.That(data.CurrentPage(), Is.EqualTo(root));
            Assert.That(data.Values["controller"], Is.EqualTo("Regular"));
            Assert.That(data.Values["action"], Is.EqualTo("Index"));
        }

        [Test]
        public void CanFindController_ForContentPage()
        {
            var data = RequestingUrl("/about/");

            Assert.That(data.CurrentItem(), Is.EqualTo(about));
            Assert.That(data.CurrentPage(), Is.EqualTo(about));
            Assert.That(data.Values["controller"], Is.EqualTo("AboutUsSectionPage"));
            Assert.That(data.Values["action"], Is.EqualTo("Index"));
        }

        [Test]
        public void CanFindController_ForContentPage_NoSlash()
        {
            var data = RequestingUrl("/about");

            Assert.That(data.CurrentItem(), Is.EqualTo(about));
            Assert.That(data.CurrentPage(), Is.EqualTo(about));
            Assert.That(data.Values["controller"], Is.EqualTo("AboutUsSectionPage"));
            Assert.That(data.Values["action"], Is.EqualTo("Index"));
        }

        [Test]
        public void RoutesOnlyToPage_WhenPart_IsPassedAsItem_InQuery()
        {
            var part = CreateOneItem<TestItem>(10, "whatever", root);
            var data = RequestingUrl("/about/?n2item=10");

            Assert.That(data.CurrentItem(), Is.EqualTo(about));
            Assert.That(data.CurrentPage(), Is.EqualTo(about));
            Assert.That(data.Values["controller"], Is.EqualTo("AboutUsSectionPage"));
            Assert.That(data.Values["action"], Is.EqualTo("Index"));
        }

        [Test]
        public void CanFindController_ForExtendingType()
        {
            RequestingUrl("/about/executives/");

            var data = route.GetRouteData(httpContext);

            Assert.That(data.CurrentItem(), Is.EqualTo(executives));
            Assert.That(data.CurrentPage(), Is.EqualTo(executives));
            Assert.That(data.Values["controller"], Is.EqualTo("ExecutiveTeam"));
            Assert.That(data.Values["action"], Is.EqualTo("Index"));
        }

        [Test]
        public void CanGet_ActionName_FromUrl()
        {
            RequestingUrl("/about/submit");

            var routeData = route.GetRouteData(httpContext);

            Assert.That(routeData, Is.Not.Null);
            Assert.That(routeData.Values["controller"], Is.EqualTo("AboutUsSectionPage"));
            Assert.That(routeData.Values["action"], Is.EqualTo("submit"));
        }

        [Test]
        public void CanRoute_ToPage_ViaController()
        {
            var part = CreateOneItem<TestItem>(10, "whatever", root);
            RequestingUrl("/Regular/?n2page=1");

            var r = routes.GetRouteData(httpContext);

            Assert.That(r.CurrentItem(), Is.EqualTo(root));
            Assert.That(r.CurrentPage(), Is.EqualTo(root));
            Assert.That(r.Values[ContentRoute.ControllerKey], Is.EqualTo("Regular"));
            Assert.That(r.Values[ContentRoute.ActionKey], Is.EqualTo("Index"));
        }

        [Test, Ignore("TODO")]
        public void CanGet_IdParameter_FromUrl()
        {
            RequestingUrl("/about/submit/123");

            var routeData = route.GetRouteData(httpContext);

            Assert.That(routeData, Is.Not.Null);
            Assert.That(routeData.Values["controller"], Is.EqualTo("AboutUsSectionPage"));
            Assert.That(routeData.Values["action"], Is.EqualTo("submit"));
            Assert.That(routeData.Values["id"], Is.EqualTo("123"));
        }
    }
}
