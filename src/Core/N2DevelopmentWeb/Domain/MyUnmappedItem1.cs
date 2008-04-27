using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace N2DevelopmentWeb.Domain
{
    [N2.Definition("My unmapped page 1"),
        N2.Integrity.RestrictParents(typeof(MyPageData), typeof(MySpecialPageData))]
    public class MyUnmappedItem1 : MyUnmappedItem
    {
    }
}
