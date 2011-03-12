using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace N2.Web.Mvc.Html
{
	public static class ContentUrlExtensions
	{
		public static string Action(this UrlHelper url, ContentItem item)
		{
			return url.Action("Index", new { item = item.ID });
		}
	}
}
