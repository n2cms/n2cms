using System;
using N2.Collections;

namespace N2.Tests.Web.Items
{
    public class CustomItem : ContentItem
    {
	    [Obsolete]
		public override ItemList GetChildren(string childZoneName)
	    {
		    return GetChildren(new ZoneFilter(childZoneName));
	    }
    }
}
