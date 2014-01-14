using N2.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Targeting.Detectors
{
    [Detector]
    public class Anonymous : DetectorBase
    {
        public override bool IsTarget(N2.Web.Targeting.TargetingContext context)
        {
            return !context.HttpContext.User.Identity.IsAuthenticated;
        }
    }
}
