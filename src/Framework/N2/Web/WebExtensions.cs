using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO.Compression;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Details;
using System.IO;

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

		internal static IDisposable GetEditableWrapper(ContentItem item, bool isEditable, string propertyName, IDisplayable displayable, TextWriter writer)
		{
			var viewEditable = displayable as IViewEditable;
			if (isEditable && (viewEditable == null || viewEditable.IsViewEditable) && item != null && displayable != null)
				return TagWrapper.Begin("div", writer, htmlAttributes: new RouteValueDictionary { { "data-id", item.ID }, { "data-path", item.Path }, { "data-property", propertyName }, { "data-displayable", displayable.GetType().Name }, { "class", "editable " + displayable.GetType().Name + " Editable" + propertyName } });
			else
				return new EmptyDisposable();
		}
	}
}
