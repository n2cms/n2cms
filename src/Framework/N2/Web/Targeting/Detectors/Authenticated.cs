using N2.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Targeting.Detectors
{
    [Detector]
    public class Authenticated : DetectorBase
    {
        public override bool IsTarget(TargetingContext context)
        {
            return context.HttpContext.User.Identity.IsAuthenticated;
        }
    }
}
