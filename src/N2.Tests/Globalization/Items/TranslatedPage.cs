using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;

namespace N2.Tests.Globalization.Items
{
	public class TranslatedPage : ContentItem
	{
	}

    public class TranslatedPageWithAuthorization : ContentItem
    {
        public bool Authorize = true;
        public override bool IsAuthorized(IPrincipal user)
        {
            return Authorize;
        }
    }
}
