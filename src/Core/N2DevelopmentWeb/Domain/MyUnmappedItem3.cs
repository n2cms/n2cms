using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace N2.TemplateWeb.Domain
{
    [N2.Definition("My unmapped page 3"),
        N2.Integrity.AvailableZone("Right", "Right"),
        N2.Integrity.AvailableZone("Content", "Content"),
        N2.Integrity.RestrictParents(typeof(MySpecialPageData))]
    public class MyUnmappedItem3 : MyUnmappedItem1
    {
    }
}
