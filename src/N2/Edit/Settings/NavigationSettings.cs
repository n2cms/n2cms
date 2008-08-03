using System;
using System.Web;
using N2.Web;

namespace N2.Edit.Settings
{
	public class NavigationSettings
	{
		private IWebContext context;

		public NavigationSettings(IWebContext context)
		{
			this.context = context;
		}

		[EditableCheckBox("Display data items", 100)]
		public bool DisplayDataItems
		{
			get { return Boolean.Parse(GetCookie(context.Request.Cookies).Value); }
			set { GetCookie(context.Response.Cookies).Value = value.ToString(); }
		}

		private HttpCookie GetCookie(HttpCookieCollection cookies)
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