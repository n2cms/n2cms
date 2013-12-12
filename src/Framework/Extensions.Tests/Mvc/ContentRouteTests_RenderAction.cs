using System.Web.Mvc;
using System.Web.Mvc.Html;
using NUnit.Framework;

namespace N2.Extensions.Tests.Mvc
{
    [TestFixture]
    public class ContentRouteTests_RenderAction : ContentRouteTests
    {
        [Test]
        public void CanRender_NonContentController()
        {
            routes.MapRoute("test", "{controller}/{action}");
            RequestingUrl("/");

            htmlHelper.RenderAction("top", "navigation");

            string renderedValues = httpContext.Response.Output.ToString();
            Assert.That(renderedValues.Contains("controller=navigation"));
            Assert.That(renderedValues.Contains("action=top"));
        }
    }

}
