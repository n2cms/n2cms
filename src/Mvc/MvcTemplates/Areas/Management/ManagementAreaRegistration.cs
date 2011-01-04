using System.Web.Mvc;
using N2.Web.Mvc;

namespace N2.Templates.Mvc.Areas.Management
{
	public class ManagementAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get { return "Management"; }
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapContentRoute<Models.AnalyticsPartBase>();
		}
	}
}
