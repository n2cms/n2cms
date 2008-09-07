using System;
using System.Collections.Generic;
using System.Web;
using N2.Definitions;
using N2.Integrity;

namespace N2.Templates.Items
{
    [Disable] // This item is added by the CommentInput thus it's disabled
    [Definition]
    [RestrictParents(typeof(AbstractPage))]
    public class CommentList : AbstractItem
    {
    }
}
