using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

namespace N2.Tests.Globalization.Items
{
    public class TranslatedPageWithAuthorization : ContentItem
    {
        public bool Authorize = true;
        public override bool IsAuthorized(IPrincipal user)
        {
            return Authorize;
        }
    }
}
