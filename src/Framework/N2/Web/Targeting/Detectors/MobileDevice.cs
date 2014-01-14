using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Targeting.Detectors
{
    [Detector]
    public class MobileDevice : DetectorBase
    {
        public override bool IsTarget(TargetingContext context)
        {
            return context.HttpContext.Request.Browser.IsMobileDevice;
        }
    }
}
