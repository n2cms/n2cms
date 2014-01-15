using N2.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Targeting.Detectors
{
    [Detector]
    public class Manager : DetectorBase
    {
        private ISecurityManager security;
        
        public Manager(ISecurityManager security)
        {
            this.security = security;
        }

        public override bool IsTarget(TargetingContext context)
        {
            return context.HttpContext.User.Identity.IsAuthenticated
                && security.IsAuthorized(context.HttpContext.User, Permission.Write);
        }
    }
}
