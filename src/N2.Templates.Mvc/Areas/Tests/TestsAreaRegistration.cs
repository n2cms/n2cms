#if DEMO
using System.Web.Mvc;
using N2.Web.Mvc;

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
				"Tests/{controller}/{action}",
				new { action = "index" },
				new { area = "Tests" }
				);
		}
	}
}
#endif