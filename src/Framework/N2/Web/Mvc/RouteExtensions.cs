using System;
using System.Linq;
using System.Web.Routing;
using N2.Engine;
using N2.Persistence;
using System.Collections.Generic;
using System.Web.Mvc;

namespace N2.Web.Mvc
{
    public static class RouteExtensions
    {
        /// <summary>Resolves the current <see cref="IEngine"/> using information stored in context or falling back to the singleton instance.</summary>
        /// <param name="routeData"></param>
        /// <returns></returns>
        public static IEngine GetEngine(RouteData routeData)
        {
            return routeData.DataTokens[ContentRoute.ContentEngineKey] as IEngine
                ?? N2.Context.Current;
        }

        /// <summary>Resolves a service from the current <see cref="IEngine"/> using information stored in context or falling back to the singleton instance.</summary>
        public static T ResolveService<T>(RouteData routeData) where T : class
        {
            return GetEngine(routeData).Resolve<T>();
        }

        /// <summary>Resolves services from the current <see cref="IEngine"/> using information stored in context or falling back to the singleton instance.</summary>
        public static IEnumerable<T> ResolveServices<T>(RouteData routeData) where T : class
        {
            return GetEngine(routeData).Container.ResolveAll<T>();
        }



        /// <summary>Applies existing or creates external content on the route data.</summary>
        /// <param name="data">The data that will receive external content.</param>
        /// <param name="family">The group to associate external content to.</param>
        /// <param name="key">The key of this particular external content.</param>
        /// <param name="url">The url on which this external content item is displayed.</param>
        /// <returns>The external content item itself.</returns>
        public static ContentItem ApplyExternalContent(this RouteData data, string family, string key, string url, Type contentType = null)
        {
            var item = ResolveService<Edit.IExternalContentRepository>(data).GetOrCreate(family, key, url, contentType);
            data.ApplyCurrentPath(new PathData(item));
            return item;
        }

        /// <summary>Applies the current path to the route data.</summary>
        public static RouteData ApplyCurrentPath(this RouteData data, PathData path)
        {
            data.Values[PathData.PathKey] = path.ToString();
            data.DataTokens[PathData.PathKey] = path;

            return data;
        }

        /// <summary>Applies the current content item and controller to the route data.</summary>
        public static RouteData ApplyCurrentPath(RouteData data, string controllerName, string actionName, PathData path)
        {
            data.Values[ContentRoute.ControllerKey] = controllerName;
            data.Values[ContentRoute.ActionKey] = actionName;
            return data.ApplyCurrentPath(path);
        }

        public static PathData CurrentPath(this RouteData routeData)
        {
            if (routeData.DataTokens.ContainsKey(PathData.PathKey))
                return routeData.DataTokens[PathData.PathKey] as PathData ?? PathData.Empty;
            
            if (routeData.DataTokens.ContainsKey("ParentActionViewContext"))
            {
                var viewContext = routeData.DataTokens["ParentActionViewContext"] as ControllerContext;
                if (viewContext != null)
                {
                    return viewContext.RouteData.CurrentPath();
                }
            }
            return PathData.Empty;
        }

        public static ContentItem CurrentItem(this RouteData routeData)
        {
            return routeData.CurrentPath().CurrentItem;
        }

        public static ContentItem CurrentPage(this RouteData routeData)
        {
            return routeData.CurrentPath().CurrentPage;
        }

        public static T CurrentItem<T>(this RouteValueDictionary data, string key, IPersister persister)
            where T: ContentItem
        {
            if (data.ContainsKey(key))
            {
                object value = data[key];
                if (value == null)
                    return null;
                
                var item = value as T;
                if (item != null)
                    return item;

                if(value is int)
                    return persister.Get((int)value) as T;

                int itemId;
                if (value is string && int.TryParse(value as string, out itemId))
                    return persister.Get(itemId) as T;
            }

            return null;
        }

        internal class QueryStringOutput : QueryStringOutput<string, object>
        {
            public QueryStringOutput(RouteData data)
                : base (data != null ? data.Values : null)
            {
            }

            public QueryStringOutput(IDictionary<string, object> values)
                : base(values)
            {
            }

        }

        internal class QueryStringOutput<TKey, TValue>
        {
            private readonly IDictionary<TKey, TValue> values;

            public QueryStringOutput(IDictionary<TKey, TValue> values)
            {
                this.values = values;
            }

            public override string ToString()
            {
                if (values == null)
                    return string.Empty;
                return string.Join("&", values.Select(kvp => kvp.Key + "=" + kvp.Value).ToArray());
            }
        }

        public static string ToQueryString<TKey, TValue>(this IDictionary<TKey, TValue> values)
        {
            return new QueryStringOutput<TKey, TValue>(values).ToString();
        }
    }
}
