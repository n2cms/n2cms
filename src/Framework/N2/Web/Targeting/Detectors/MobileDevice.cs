using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Targeting.Detectors
{
	public class MobileDevice : DetectorBase
	{
		public override bool IsTarget(TargetingContext context)
		{
			return context.HttpContext.Request.Browser.IsMobileDevice;
		}
	}
}
