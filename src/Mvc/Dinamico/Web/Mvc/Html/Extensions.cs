using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Web;
using System.Web.UI;

namespace N2.Web.Mvc.Html
{
	public static class Extensions
	{
		public static IHtmlString ToHtmlString(this object instance)
		{
			return new HtmlString(instance.ToString());
		}

		public static void RenderControlPanel(this HtmlHelper html)
		{
			//TODO
			var p = new Page();
			var cp = new N2.Web.UI.WebControls.ControlPanel();
			cp.CurrentItem = html.CurrentItem();
			p.Controls.Add(cp);

			cp.RenderControl(new HtmlTextWriter(html.ViewContext.Writer));
		}
	}
}