using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO.Compression;
using System.Web.Mvc;
using System.Web.Routing;

namespace N2.Web
{
	public static class WebExtensions
	{
		public static bool TrySetCompressionFilter(this HttpContext context)
		{
			string acceptEncoding = context.Request.Headers["Accept-Encoding"] ?? "";
			if (acceptEncoding.Contains("gzip"))
			{
				context.Response.Filter = new GZipStream(context.Response.Filter, CompressionMode.Compress);
				context.Response.AppendHeader("Content-Encoding", "gzip");
				return true;
			}
			else if (acceptEncoding.Contains("deflate"))
			{
				context.Response.Filter = new DeflateStream(context.Response.Filter, CompressionMode.Compress);
				context.Response.AppendHeader("Content-Encoding", "deflate");
				return true;
			}
			return false;
		}

		internal static void MergeAttributes(this TagBuilder tag, object htmlAttributes)
		{
			if (htmlAttributes == null)
				return;
			tag.MergeAttributes(new RouteValueDictionary(htmlAttributes));
		}

		internal static TagBuilder AddAttributeUnlessEmpty(this TagBuilder tag, string attribute, object value)
		{
			if (value == null)
				return tag;

			return tag.AddAttributeUnlessEmpty(attribute, value.ToString());
		}

		internal static TagBuilder AddAttributeUnlessEmpty(this TagBuilder tag, string attribute, string value)
		{
			if (string.IsNullOrEmpty(value))
				return tag;

			tag.Attributes[attribute] = value;

			return tag;
		}
	}
}
