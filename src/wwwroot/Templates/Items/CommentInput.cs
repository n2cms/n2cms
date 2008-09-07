using System;
using System.Collections.Generic;
using System.Web;
using N2.Integrity;

namespace N2.Templates.Items
{
    [Definition]
    [RestrictParents(typeof(AbstractPage))]
    [AllowedZones(Zones.Content, Zones.RecursiveBelow)]
    public class CommentInput : AbstractItem
    {
    }
}
