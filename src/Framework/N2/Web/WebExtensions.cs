﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO.Compression;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Details;
using System.IO;
using System.Web.UI;
using System.Collections;

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
		/// <summary>Converts a string to an <see cref="Url"/></summary>
		/// <param name="url">The url string.</param>
		/// <returns>The string parsed into an Url.</returns>
		public static Url ToUrl(this string url)
		{
			return Url.Parse(url);
		}

		public static Tree OpenTo(this Tree treeBuilder, ContentItem item)
		{
			var items = new HashSet<ContentItem>(Find.EnumerateParents(item));
			return treeBuilder.ClassProvider(null, n => items.Contains(n.Current) || n.Current == item ? "open" : string.Empty);
		}

		internal static Control AddTo(this ILinkBuilder builder, Control container)
		{
			var c = builder.ToControl();
			if (c != null)
				container.Controls.Add(c);
			return c;
		}

        public static string ToJson(this object value)
        {
            using (var sw = new StringWriter())
            {
                value.ToJson(sw);
                return sw.ToString();
            }
        }

        public static void ToJson(this object value, TextWriter sw)
        {
            new JsonWriter(sw).Write(value);
        }

		public static string ResolveUrlTokens(this string url)
		{
			return Url.ResolveTokens(url);
		}

		/// <summary>Creates the url used for rewriting friendly urls to the url of a template.</summary>
		/// <param name="path">The path containing item information to route.</param>
		/// <returns>The path to a template.</returns>
		/// <remarks>This method may throw <see cref="TemplateNotFoundException"/> if the template cannot be computed.</remarks>
		public static Url GetRewrittenUrl(this PathData path)
		{
			if (path.IsEmpty() || string.IsNullOrEmpty(path.TemplateUrl))
				return null;

			if (path.CurrentPage.IsPage)
			{
				Url url = Url.Parse(path.TemplateUrl)
					.UpdateQuery(path.QueryParameters)
					.SetQueryParameter(PathData.PageQueryKey, path.CurrentPage.ID);
				if (!string.IsNullOrEmpty(path.Argument))
					url = url.SetQueryParameter("argument", path.Argument);

				return url.ResolveTokens();
			}

			for (ContentItem ancestor = path.CurrentItem.Parent; ancestor != null; ancestor = ancestor.Parent)
				if (ancestor.IsPage)
					return ancestor.FindPath(PathData.DefaultAction).GetRewrittenUrl()
						.UpdateQuery(path.QueryParameters)
						.SetQueryParameter(PathData.ItemQueryKey, path.CurrentItem.ID);

			if (path.CurrentItem.VersionOf.HasValue)
				return path.CurrentItem.VersionOf.FindPath(PathData.DefaultAction).GetRewrittenUrl()
					.UpdateQuery(path.QueryParameters)
					.SetQueryParameter(PathData.ItemQueryKey, path.CurrentItem.ID);

			throw new TemplateNotFoundException(path.CurrentItem);
		}
	}
}
