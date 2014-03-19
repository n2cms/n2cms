using System;
using System.Diagnostics;
using System.IO;
using N2.Configuration;
using N2.Persistence;
using N2.Plugin;
using N2.Edit.Versioning;

namespace N2.Web
{
    /// <summary>
    /// Creates unique managementUrls for items and finds the corresponding item from
    /// such an url.
    /// </summary>
    public class UrlParser : IUrlParser
    {
        private readonly Engine.Logger<UrlParser> logger;
        protected readonly IPersister persister;
        protected readonly IHost host;
        protected readonly IWebContext webContext;
        readonly bool ignoreExistingFiles;

        public event EventHandler<PageNotFoundEventArgs> PageNotFound;
        public event EventHandler<UrlEventArgs> BuildingUrl;
        public event EventHandler<UrlEventArgs> BuiltUrl;

        /// <summary>Is set to the current database connection status.</summary>
        protected bool IsOnline { get; set; }

        public UrlParser(IPersister persister, IWebContext webContext, IHost host, N2.Plugin.ConnectionMonitor connections, HostSection config)
        {
            if (host == null) throw new ArgumentNullException("host");

            IsOnline = connections.IsConnected ?? true;
            connections.Online += (s, a) => IsOnline = true;
            connections.Offline += (s, a) => IsOnline = false;

            this.persister = persister;
            this.webContext = webContext;
            this.host = host;

            ignoreExistingFiles = config.Web.IgnoreExistingFiles;
        }

        /// <summary>Parses the current url to retrieve the current page.</summary>
        public ContentItem CurrentPage
        {
            get { return webContext.CurrentPage ?? (webContext.CurrentPage = FindPath(webContext.Url).CurrentPage); }
        }

        /// <summary>Gets the current start page.</summary>
        public virtual ContentItem StartPage
        {
            get { return IsOnline ? persister.Repository.Get(host.CurrentSite.StartPageID) : null; }
        }

        /// <summary>Gets or sets the default content document name. This is usually "/Default.aspx".</summary>
        public string DefaultDocument
        {
            get { return Url.DefaultDocument; }
            set { Url.DefaultDocument = value; }
        }

        [Obsolete("Use FindPath")]
        /// <summary>Finds the path associated with an url.</summary>
        /// <param name="url">The url to the template to locate.</param>
        /// <param name="startNode">The node to start finding path from if none supplied will start from StartNode</param>
        /// <param name="remainingPath">The remaining path to search</param>
        /// <returns>A PathData object. If no template was found the object will have empty properties.</returns>
        public PathData ResolvePath(Url url, ContentItem startNode = null, string remainingPath = null)
        {
            return FindPath(url, startNode, remainingPath);
        }

        /// <summary>Finds the path associated with an url.</summary>
        /// <param name="url">The url to the template to locate.</param>
        /// <param name="startNode">The node to start finding path from if none supplied will start from StartNode</param>
        /// <param name="remainingPath">The remaining path to search</param>
        /// <returns>A PathData object. If no template was found the object will have empty properties.</returns>
        public PathData FindPath(Url url, ContentItem startNode = null, string remainingPath = null)
        {
            if (url == null) return PathData.Empty;
            if (!IsOnline) return PathData.Empty;

            Url requestedUrl = url;
            ContentItem item = TryLoadingFromQueryString(requestedUrl, PathData.ItemQueryKey);
            ContentItem page = TryLoadingFromQueryString(requestedUrl, PathData.PageQueryKey);

            if (page != null)
            {
                var directPath = page.FindPath(requestedUrl["action"] ?? PathData.DefaultAction)
                    .SetArguments(requestedUrl["arguments"])
                    .UpdateParameters(requestedUrl.GetQueries());

                var directData = UseItemIfAvailable(item, directPath);
                // check whether to rewrite requests with page in query string since this might be already rewritten
                directData.IsRewritable &= !string.Equals(url.ApplicationRelativePath, directData.TemplateUrl, StringComparison.InvariantCultureIgnoreCase);
                return directData;
            }

            ContentItem startPage = startNode ?? GetStartPage(requestedUrl);
            if (startPage == null)
                return PathData.Empty;

            string path = remainingPath ?? Url.ToRelative(requestedUrl.Path).TrimStart('~');
            PathData data = startPage.FindPath(path).UpdateParameters(requestedUrl.GetQueries());

            if (data.IsEmpty())
            {
                if (!string.IsNullOrEmpty(DefaultDocument) && path.EndsWith(DefaultDocument, StringComparison.OrdinalIgnoreCase))
                {
                    // Try to find path without default document.
                    data = StartPage
                        .FindPath(StripDefaultDocument(path))
                        .UpdateParameters(requestedUrl.GetQueries());
                }

                if (data.IsEmpty())
                {
                    // Allow user code to set path through event
                    if (PageNotFound != null)
                    {
                        PageNotFoundEventArgs args = new PageNotFoundEventArgs(requestedUrl);
                        args.AffectedPath = data;
                        PageNotFound(this, args);
                        data = args.AffectedPath;
                    }
                }
            }

            data.Ignore = !IgnoreExisting(webContext.HttpContext.Request.PhysicalPath);
            data = UseItemIfAvailable(item, data);
            return data;
        }

        private static PathData UseItemIfAvailable(ContentItem item, PathData data)
        {
            if (item != null)
            {
                data.CurrentPage = data.CurrentItem;
                data.CurrentItem = item;
            }
            return data;
        }

        /// <summary>May be overridden to provide custom start page depending on authority.</summary>
        /// <param name="url">The host name and path information.</param>
        /// <returns>The configured start page.</returns>
        protected virtual ContentItem GetStartPage(Url url)
        {
            return StartPage;
        }

        bool IgnoreExisting(string physicalPath)
        {
            // N2 has a history of requiring the start page's template to be located at /Default.aspx.
            // Since a previous version this is no longer required with the consequence of /Default.aspx
            // beeing required only for igniting an asp.net web request when accessing /. With the new
            // behaviour access to the default document (/ or /Default.aspx) will be rewritten to which-
            // ever template the current start page specifies. The previous behaviour can be restored
            // by configuring n2 to ignore existing files.
            return ignoreExistingFiles || (!File.Exists(physicalPath) && !Directory.Exists(physicalPath));
        }

        bool IsDefaultDocument(string path)
        {
            return path.Equals(DefaultDocument, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>Finds an item by traversing names from the start page.</summary>
        /// <param name="url">The url that should be traversed.</param>
        /// <returns>The content item matching the supplied url.</returns>
        public virtual ContentItem Parse(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;

            ContentItem startingPoint = GetStartPage(url);
            if (startingPoint == null)
                return null;

            return TryLoadingFromQueryString(url, PathData.ItemQueryKey, PathData.PageQueryKey) 
                ?? Parse(startingPoint, url);
        }

        #region Parse Helper Methods
        protected virtual ContentItem TryLoadingFromQueryString(string url, params string[] parameters)
        {
            int? itemID = FindQueryStringReference(url, parameters);
            if (itemID.HasValue)
                return persister.Get(itemID.Value);
            return null;
        }

        protected virtual ContentItem Parse(ContentItem current, Url url)
        {
            if (current == null) throw new ArgumentNullException("current");
            logger.Debug("Parsing " + url);
            var path = CleanUrl(url.PathAndQuery);

            if (path.Length == 0)
                return current;
            
            return current.GetChild(path) 
                ?? NotFoundPage(url);
        }

        /// <summary>Returns a page when  no page is found. This method will return the start page if the url matches the default content page property.</summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected virtual ContentItem NotFoundPage(string url)
        {
            if (IsDefaultDocument(url))
            {
                return StartPage;
            }
            logger.Debug("No content at: " + url);

            PageNotFoundEventArgs args = new PageNotFoundEventArgs(url);
            if (PageNotFound != null)
                PageNotFound(this, args);
            return args.AffectedItem;
        }

        private string CleanUrl(string url)
        {
            url = Url.PathPart(url);
            url = Url.ToRelative(url);
            url = url.TrimStart('~', '/');
            if (url.EndsWith(DefaultDocument))
                url = url.Substring(0, url.Length - DefaultDocument.Length);
            return url;
        }

        private int? FindQueryStringReference(string url, params string[] parameters)
        {
            string queryString = Url.QueryPart(url);
            if (!string.IsNullOrEmpty(queryString))
            {
                string[] queries = queryString.Split('&');

                foreach (string parameter in parameters)
                { 
                    int parameterLength = parameter.Length + 1;
                    foreach (string query in queries)
                    {
                        if (query.StartsWith(parameter + "=", StringComparison.InvariantCultureIgnoreCase))
                        {
                            int id;
                            if (int.TryParse(query.Substring(parameterLength), out id))
                            {
                                return id;
                            }
                        }
                    }
                }
            }
            return null;
        }
        #endregion

        /// <summary>Calculates an item url by walking it's parent path.</summary>
        /// <param name="item">The item whose url to compute.</param>
        /// <returns>A friendly url to the supplied item.</returns>
        public virtual Url BuildUrl(ContentItem item)
        {
            if (item == null) throw new ArgumentNullException("item");

            if (item.VersionOf.HasValue)
            {
                ContentItem version = item.VersionOf;
                if (version != null)
                    return BuildUrl(version).SetQueryParameter(PathData.VersionIndexQueryKey, item.VersionIndex);
            }
            else if (item.ID == 0)
            {
                var page = Find.ClosestPage(item);
                if (page != null && page != item)
                {
                    return BuildUrl(page)
                        .SetQueryParameter(PathData.VersionIndexQueryKey, page.VersionIndex)
                        .SetQueryParameter(PathData.VersionKeyQueryKey, item.GetVersionKey());
                }
            }

            var current = Find.ClosestPage(item);

            // no page found, throw
            if (current == null) throw new N2Exception("Cannot build url to data item '{0}' with no containing page item.", item);

            Url url = BuildPageUrl(current, ref current);

            if (current == null)
                // no start page found, use rewritten url
                return item.FindPath(PathData.DefaultAction).GetRewrittenUrl();

            if (!item.IsPage)
                // the item was not a page, add this information as a query string
                url = url.AppendQuery(PathData.ItemQueryKey, item.ID);

            var absoluteUrl = Url.ToAbsolute("~" + url);
            if (BuiltUrl != null)
            {
                var args = new UrlEventArgs(item) { Url = absoluteUrl };
                BuiltUrl(this, args);
                return args.Url;
            }
            else
                return absoluteUrl;
        }

        protected Url BuildPageUrl(ContentItem page, ref ContentItem current)
        {
            Url url = "/";
            if (!host.IsStartPage(current))
            {
                if (BuildingUrl != null)
                {
                    // build path until a start page
                    while (current != null && !host.IsStartPage(current))
                    {
                        var args = new UrlEventArgs(current);
                        BuildingUrl(this, args);
                        if (!string.IsNullOrEmpty(args.Url))
                        {
                            url = Url.ToRelative(args.Url).TrimStart('~').ToUrl().AppendSegment(url);
                            //url = url.PrependSegment(Url.ToRelative(args.Url).TrimStart('~'));
                            break;
                        }
                        url = url.PrependSegment(current.Name);
                        current = current.Parent;
                    }
                }
                else
                {
                    while (current != null && !host.IsStartPage(current))
                    {
                        url = url.PrependSegment(current.Name);
                        current = current.Parent;
                    }
                }
                url = url.SetExtension(page.Extension);
            }
            return url;
        }

        /// <summary>Checks if an item is startpage or root page</summary>
        /// <param name="item">The item to compare</param>
        /// <returns>True if the item is a startpage or a rootpage</returns>
        public virtual bool IsRootOrStartPage(ContentItem item)
        {
            return item.ID == host.CurrentSite.RootItemID || host.IsStartPage(item);
        }

        /// <summary>Removes a trailing Default.aspx from an URL.</summary>
        /// <param name="path">A URL path without query strings from which to remove any trailing Default.aspx.</param>
        /// <returns>The same path or one stripped of the remaining default document segment.</returns>
        public string StripDefaultDocument(string path)
        {
            if (path.EndsWith(DefaultDocument, StringComparison.OrdinalIgnoreCase))
                return path.Substring(0, path.Length - DefaultDocument.Length);
            return path;
        }
    }
}
