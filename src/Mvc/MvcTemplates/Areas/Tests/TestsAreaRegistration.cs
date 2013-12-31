#if DEMO
using System.Web.Mvc;
using N2.Web.Mvc;
using System.Web.Routing;

namespace N2.Templates.Mvc.Areas.Tests
{
    public class TestsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "Tests"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapContentRoute<Models.TestItemBase>();
            context.MapRoute("hello",
                "Tests/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new { area = new NonContentConstraint() },
                new [] { "N2.Templates.Mvc.Areas.Tests.Controllers" }
                );
        }
    }
}
#endif
