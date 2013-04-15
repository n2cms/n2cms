﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using N2.Definitions;
using N2.Configuration;
using N2.Edit;
using N2.Edit.Versioning;

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

			tag.Attributes[attribute] = HttpUtility.HtmlEncode(value);

			return tag;
		}

		internal static IDisposable GetEditableWrapper(ContentItem item, bool isEditable, string propertyName, IDisplayable displayable, TextWriter writer)
		{
			
			var viewEditable = displayable as IViewEditable;
			if (isEditable && (viewEditable == null || viewEditable.IsViewEditable) && item != null && displayable != null)
				return TagWrapper.Begin("div", writer, htmlAttributes: new RouteValueDictionary { { "data-id", item.ID }, { "data-path", item.Path }, { "data-property", propertyName }, { "data-displayable", displayable.GetType().Name }, { "data-versionIndex", item.VersionIndex }, { "data-versionKey", item.GetVersionKey() }, { "class", "editable " + displayable.GetType().Name + " Editable" + propertyName }, { "title", (displayable is IEditable) ? (displayable as IEditable).Title : displayable.Name } });
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
					.UpdateQuery(path.QueryParameters);
				if (path.CurrentPage.ID == 0 && path.CurrentPage.VersionOf.HasValue)
					url = url.SetQueryParameter(PathData.PageQueryKey, path.CurrentPage.VersionOf.ID.Value)
						.SetQueryParameter(PathData.VersionIndexQueryKey, path.CurrentPage.VersionIndex);
				else 
					url = url.SetQueryParameter(PathData.PageQueryKey, path.CurrentPage.ID);
				
				if (!string.IsNullOrEmpty(path.Argument))
					url = url.SetQueryParameter("argument", path.Argument);

				return url.ResolveTokens();
			}

			for (ContentItem ancestor = path.CurrentItem.Parent; ancestor != null; ancestor = ancestor.Parent)
			{
				if (ancestor.IsPage)
				{
					var url = ancestor.FindPath(PathData.DefaultAction).GetRewrittenUrl()
						.UpdateQuery(path.QueryParameters);
					if (path.CurrentItem.VersionOf.HasValue)
						return url.SetQueryParameter(PathData.ItemQueryKey, path.CurrentItem.VersionOf.ID.Value)
							.SetQueryParameter(PathData.VersionIndexQueryKey, ancestor.VersionIndex);
					else
						return url.SetQueryParameter(PathData.ItemQueryKey, path.CurrentItem.ID);
				}
			}
			if (path.CurrentItem.VersionOf.HasValue)
				return path.CurrentItem.VersionOf.FindPath(PathData.DefaultAction).GetRewrittenUrl()
					.UpdateQuery(path.QueryParameters)
					.SetQueryParameter(PathData.ItemQueryKey, path.CurrentItem.ID);

			return null;
		}

		public static bool IsFlagSet<T>(this T value, T flag) where T : struct
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException(string.Format("'{0}' is not Enum type", typeof(T).FullName));
			}

			var flagValue = Convert.ToInt64(flag);
			return flagValue == 0 || (Convert.ToInt64(value) & flagValue) != 0;
		}

		internal static string ViewPreferenceQueryString = "view";
		internal static string DraftQueryValue = ViewPreference.Draft.ToString().ToLower();
		internal static string PublishedQueryValue = ViewPreference.Published.ToString().ToLower();
		public static ViewPreference GetViewPreference(this HttpContextBase context, ViewPreference defaultPreference = ViewPreference.Published)
		{
			if (context.Request[ViewPreferenceQueryString] == DraftQueryValue)
				return ViewPreference.Draft;
			else if (context.Request[ViewPreferenceQueryString] == PublishedQueryValue)
				return ViewPreference.Published;
			else
				return defaultPreference;
		}

		public static Url AppendViewPreference(this Url url, ViewPreference preference, ViewPreference? defaultPreference = null)
		{
			if (preference == defaultPreference)
				return url;
			else
				return url.SetQueryParameter(ViewPreferenceQueryString, preference.ToString().ToLower());
		}

		public static NameValueCollection ToNameValueCollection(this IDictionary<string, string> queryString)
		{
			var nvc = new NameValueCollection();
			foreach (var kvp in queryString)
			{
				nvc[kvp.Key] = kvp.Value;
			}

			return nvc;
		}

		public static ContentItem ParseVersion(this ContentVersionRepository versionRepository, string versionIndexParameterValue, string versionKey, ContentItem masterVersion)
		{
			var path = new PathData(Find.ClosestPage(masterVersion), masterVersion);
			if (TryParseVersion(versionRepository, versionIndexParameterValue, versionKey, path))
				return path.CurrentItem;
			return null;
		}

		public static bool TryParseVersion(this ContentVersionRepository versionRepository, string versionIndexParameterValue, string versionKey, PathData path)
		{
			if (!path.IsEmpty() && !string.IsNullOrEmpty(versionIndexParameterValue))
			{
				int versionIndex = int.Parse(versionIndexParameterValue);
				var version = versionRepository.GetVersion(path.CurrentPage, versionIndex);
				return path.TryApplyVersion(version, versionKey);
			}
			return false;
		}

		public static bool TryApplyVersion(this PathData path, ContentVersion version, string versionKey)
		{
			if (version != null)
			{
				if (!string.IsNullOrEmpty(versionKey))
				{
					var item = version.Version.FindDescendantByVersionKey(versionKey);
					if (item != null)
					{
						path.CurrentPage = version.Version;
						path.CurrentItem = item;
						return true;
					}
				}

				if (path.CurrentItem.IsPage)
				{
					path.CurrentPage = null;
					path.CurrentItem = version.Version;
				}
				else
				{
					path.CurrentPage = version.Version;
					path.CurrentItem = version.Version.FindDescendantByVersionKey(versionKey)
						?? version.Version.FindPartVersion(path.CurrentItem);
				}

				return true;
			}
			return false;
		}
	}
}
