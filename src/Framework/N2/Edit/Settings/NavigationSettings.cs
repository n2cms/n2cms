using System;
using System.Web;
using N2.Engine;
using N2.Web;

namespace N2.Edit.Settings
{
    /// <summary>
    /// Abstracts the storage of user display data items settings.
    /// </summary>
    [Service]
    public class NavigationSettings
    {
        private readonly IWebContext context;

        public NavigationSettings(IWebContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Whether the user has chosen to display data items.
        /// </summary>
        public virtual bool DisplayDataItems
        {
            get { return Boolean.Parse(GetCookie(context.HttpContext.Request.Cookies).Value); }
            set { GetCookie(context.HttpContext.Response.Cookies).Value = value.ToString(); }
        }

        private static HttpCookie GetCookie(HttpCookieCollection cookies)
        {
            HttpCookie ddi = cookies["DDI"];
            if (ddi == null)
            {
                ddi = new HttpCookie("DDI", false.ToString());
                cookies.Add(ddi);
            }
            return ddi;
        }
    }
}
