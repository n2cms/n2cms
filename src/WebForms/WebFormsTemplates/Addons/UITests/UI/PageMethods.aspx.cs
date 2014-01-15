using System;
using System.Web.Services;
using System.Web.UI;

namespace N2.Addons.UITests.UI
{
    public partial class PageMethods : Page
    {
        [WebMethod]
        public static string GetCurrentDate()
        {
            return DateTime.Now.ToString();
        }
    }
}
