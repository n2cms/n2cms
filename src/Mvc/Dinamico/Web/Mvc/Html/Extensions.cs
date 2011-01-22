using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Web;
using System.Web.UI;
using N2.Engine;
using N2.Definitions;
using System.Web.Routing;
using N2.Plugin;
using N2.Edit;
using N2.Web.UI.WebControls;
using System.IO;
using System.Text;
using N2.Web.Parts;

namespace N2.Web.Mvc.Html
{
	public static class Extensions
	{
		public static IHtmlString ToHtmlString(this object instance)
		{
			return new HtmlString(instance.ToString());
		}
	}
}