using System;
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
using System.Web.Script.Serialization;
using N2.Engine;

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
			var page = Find.ClosestPage(item) ?? item;
            var viewEditable = displayable as IViewEditable;
            if (isEditable && (viewEditable == null || viewEditable.IsViewEditable) && item != null && displayable != null)
                return TagWrapper.Begin("div", writer, htmlAttributes: new RouteValueDictionary { { "data-id", item.ID }, { "data-path", item.Path }, { "data-property", propertyName }, { "data-displayable", displayable.GetType().Name }, { "data-versionIndex", page.VersionIndex }, { "data-versionKey", item.GetVersionKey() }, { "class", "editable " + displayable.GetType().Name + " Editable" + propertyName }, { "title", (displayable is IEditable) ? (displayable as IEditable).Title : displayable.Name } });
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

		public static string ToJson(this object value, bool dateCompatibility = false)
        {
            using (var sw = new StringWriter())
            {
                value.ToJson(sw, dateCompatibility);
                return sw.ToString();
            }
        }
        
        public static void ToJson(this object value, TextWriter sw, bool dateCompatibility = false)
        {
			new JsonWriter(sw, dateCompatibility).Write(value);
        }

		public static void WriteJson(this HttpResponse response, object value, bool dateCompatibility = false)
        {
            response.ContentType = "application/json";
            value.ToJson(response.Output, dateCompatibility);
        }

		public static void WriteJson(this HttpResponseBase response, object value, bool dateCompatibility = false)
        {
            response.ContentType = "application/json";
            value.ToJson(response.Output, dateCompatibility);
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
        public static ViewPreference GetViewPreference(this HttpContextBase httpContext, ViewPreference defaultPreference = ViewPreference.Published)
        {
            if (httpContext == null)
                return defaultPreference;

            string viewPreference = httpContext.Request[ViewPreferenceQueryString];
            if (DraftQueryValue.Equals(viewPreference, StringComparison.InvariantCultureIgnoreCase))
                return ViewPreference.Draft;
            else if (PublishedQueryValue.Equals(viewPreference, StringComparison.InvariantCultureIgnoreCase))
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
			{
				return path.CurrentItem;
			}
            return null;
        }

        public static bool TryParseVersion(this ContentVersionRepository versionRepository, string versionIndexParameterValue, string versionKey, PathData path)
        {
            if (!path.IsEmpty() && !string.IsNullOrEmpty(versionIndexParameterValue))
            {
                int versionIndex = int.Parse(versionIndexParameterValue);
                var version = versionRepository.GetVersion(path.CurrentPage, versionIndex);
				return path.TryApplyVersion(version, versionKey, versionRepository);
            }
            return false;
        }

        public static bool TryApplyVersion(this PathData path, ContentVersion version, string versionKey, ContentVersionRepository repository)
        {
            if (version != null)
            {
				var page = repository.DeserializeVersion(version);
				if (!string.IsNullOrEmpty(versionKey))
                {
                    var item = page.FindDescendantByVersionKey(versionKey);
                    if (item != null)
                    {
                        path.CurrentPage = page;
                        path.CurrentItem = item;
                        return true;
                    }
                }

                if (path.CurrentItem.IsPage)
                {
                    path.CurrentPage = null;
					path.CurrentItem = page;
                }
                else
                {
					path.CurrentPage = page;
					path.CurrentItem = page.FindDescendantByVersionKey(versionKey)
						?? page.FindPartVersion(path.CurrentItem);
                }

                return true;
            }
            return false;
        }

        public static Func<string, string> GetRequestValueAccessor(this HttpContext context)
        {
            return new HttpContextWrapper(context).GetRequestValueAccessor();
        }

        public static Func<string, string> GetRequestValueAccessor(this HttpContextBase context)
        {
            if (context.Request.HttpMethod == "POST" && context.Request.ContentType.StartsWith("application/json") && context.Request.ContentLength > 0)
            {
                var json = GetOrDeserializeRequestStreamJsonDictionary<object>(context);
                if (json == null)
                    return (key) => context.Request[key];

                return (key) =>
                {
                    if (json.ContainsKey(key))
                        return json[key] != null ? Convert.ToString(json[key]) : null;
                    return context.Request[key];
                };

            }
            else
                return (key) => context.Request[key];
        }

		internal static T GetOrDeserializeRequestStreamJson<T>(this HttpContextBase context)
			where T : class
		{
			T deserializedObject = context.Items["CachedRequestStream"] as T;
			if (deserializedObject == null)
				context.Items["CachedRequestStream"] = deserializedObject = DeserialiseJson<T>(context.Request.InputStream);
			return deserializedObject;
		}

		internal static object GetOrDeserializeRequestStreamJson(this HttpContextBase context, Type targetType)
		{
			object deserializedObject = context.Items["CachedRequestStream"];
			if (deserializedObject == null)
				context.Items["CachedRequestStream"] = deserializedObject = DeserialiseJson(context.Request.InputStream, targetType);
			return deserializedObject;
		}

        internal static IDictionary<string, T> GetOrDeserializeRequestStreamJsonDictionary<T>(this HttpContextBase context)
        {
            var json = context.Items["CachedRequestStream"] as IDictionary<string, T>;
            if (json == null)
                context.Items["CachedRequestStream"] = json = DeserialiseJsonDictionary<T>(context.Request.InputStream);
            return json;
        }

		public static T DeserialiseJson<T>(this Stream stream)
		{
			using (var sr = new StreamReader(stream))
			{
				var body = sr.ReadToEnd();
				return new JavaScriptSerializer().Deserialize<T>(body);
			}
		}

		internal static object DeserialiseJson(this Stream stream, Type targetType)
		{
			using (var sr = new StreamReader(stream))
			{
				var body = sr.ReadToEnd();
				return new JavaScriptSerializer().Deserialize(body, targetType);
			}
		}

        public static IDictionary<string, T> DeserialiseJsonDictionary<T>(this Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                var body = sr.ReadToEnd();
                return new JavaScriptSerializer().Deserialize<Dictionary<string, T>>(body);
            }
        }

        public static Url AppendSelection(this Url url, ContentItem item)
        {
            if (item.ID != 0)
                // most published pages & parts
                return url.AppendQuery(SelectionUtility.SelectedQueryKey, item.Path)
                    .AppendQuery(PathData.ItemQueryKey, item.ID);
            
            if (item.VersionOf.HasValue)
                // versions of published pages and parts
                return url.AppendQuery(SelectionUtility.SelectedQueryKey, item.VersionOf.Path)
                    .AppendQuery(PathData.VersionIndexQueryKey, item.VersionIndex);
    
            if (!item.IsPage)
            {
                var page = N2.Find.ClosestPage(item);
                if (page != null)
                    // new parts
                    return url.AppendQuery(SelectionUtility.SelectedQueryKey, page.Path)
                        .AppendQuery(PathData.VersionIndexQueryKey, page.VersionIndex)
                        .AppendQuery(PathData.VersionKeyQueryKey, item.GetVersionKey());
            }

            return url.AppendQuery(SelectionUtility.SelectedQueryKey, item.Path);
        }

        internal static HttpContextBase GetHttpContextBase(this HttpContext httpContext)
        {
            var ctx = httpContext.Items["N2.HttpContextBase"] as HttpContextBase;
            if (ctx == null)
                httpContext.Items["N2.HttpContextBase"] = ctx = new HttpContextWrapper(httpContext);
            return ctx;
        }

        internal static IEngine GetEngine(this HttpContext context)
        {
            return context.GetHttpContextBase().GetEngine();
        }

        internal static IEngine GetEngine(this HttpContextBase context, IEngine engine = null)
        {
            if (engine != null)
                return engine;

            engine = context.Items["N2.Engine"] as IEngine;
            if (engine == null)
                context.Items["N2.Engine"] = engine = N2.Context.Current;
            return engine;
        }

        internal static void SetEngine(this HttpContextBase context, IEngine engine)
        {
            context.Items["N2.Engine"] = engine;
        }

        internal static bool IsEmpty(this Url url)
        {
            return url == null || string.IsNullOrEmpty(url);
        }
    }
}
