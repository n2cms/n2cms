using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using N2.Edit;
using N2.Persistence;

namespace N2.Web
{
    /// <summary>
    /// A data carrier used to pass data about a found content item and 
    /// it's template from the content item to the url rewriter.
    /// </summary>
    [Serializable, DebuggerDisplay("PathData ({CurrentItem})")]
    public class PathData : ICloneable
    {
        #region Static
        public const string DefaultAction = "";

        /// <summary>An empty path. This probably indicates that the path didn't correspond to an item in the hierarchy.</summary>
        public static PathData Empty
        {
            get { return new PathData { IsRewritable = false }; }
        }

        /// <summary>The path didn't correspond to a content item. The caller may use the last found item and remaining url to take action.</summary>
		/// <param name="reportedBy">The last item reporting no match.</param>
        /// <param name="remainingUrl">The remaining url when no match was found.</param>
        /// <returns>A an empty path data with additional information.</returns>
        public static PathData None(ContentItem reportedBy, string remainingUrl)
        {
            return new PathData { StopItem = reportedBy, Argument = remainingUrl, IsRewritable = false };
        }

        /// <summary>Creates a path that isn't rewritten to it's template.</summary>
        /// <param name="item">The item associated with path.</param>
        /// <returns>A path data that is not rewritten.</returns>
        public static PathData NonRewritable(ContentItem item)
        {
            return new PathData(item) { IsRewritable = false };
        }

        static string itemQueryKey = "n2item";
		static string pageQueryKey = "n2page";
        static string partQueryKey = "n2part";
        static string pathDataKey = "n2path";
        static string versionIndexQueryKey = "n2versionIndex";
        static string versionKeyQueryKey = "n2versionKey";

        /// <summary>The version key query key.</summary>
        public static string VersionKeyQueryKey
        {
            get { return versionKeyQueryKey; }
            set { versionKeyQueryKey = value; }
        }

        /// <summary>The version index query key.</summary>
        public static string VersionIndexQueryKey
        {
            get { return versionIndexQueryKey; }
            set { versionIndexQueryKey = value; }
        }

        /// <summary>The item query string parameter.</summary>
        public static string ItemQueryKey
        {
            get { return itemQueryKey; }
            set { itemQueryKey = value; }
        }

        /// <summary>The page query string parameter.</summary>
        public static string PageQueryKey
        {
            get { return pageQueryKey; }
            set { pageQueryKey = value; }
        }

        /// <summary>The part query string parameter.</summary>
        public static string PartQueryKey
        {
            get { return partQueryKey; }
            set { partQueryKey = value; }
        }

        /// <summary>A key used to override the path when rendering sub-actions.</summary>
        public static string PathOverrideKey { get { return "Override." + PathKey; } }

        /// <summary>Key used to access path data from context dictionaries.</summary>
        public static string PathKey
        {
            get { return pathDataKey; }
            set { pathDataKey = value; }
        }

        /// <summary>The selection query string parameter.</summary>
        public static string SelectedQueryKey
        {
            get { return SelectionUtility.SelectedQueryKey; }
            set { SelectionUtility.SelectedQueryKey = value; }
        }
        #endregion


        ContentItem currentPage;
        ContentItem currentItem;
        ContentItem stopItem;
        IPersister persister;
        
        public PathData(ContentItem item, string templateUrl, string action, string arguments)
            : this()
        {
            CurrentItem = item;
            TemplateUrl = templateUrl;
            Action = action;
            Argument = arguments;
        }

        public PathData(ContentItem item)
            : this(item, null, DefaultAction, string.Empty)
        {
        }

        public PathData(ContentItem page, ContentItem part)
            : this(part ?? page, null, DefaultAction, string.Empty)
        {
            if (part != null)
            {
                CurrentPage = page;
            }
        }

        public PathData(ContentItem item, string templateUrl)
            : this(item, templateUrl, DefaultAction, string.Empty)
        {
        }

        public PathData(int id, int pageID, string path, string templateUrl, string action, string arguments, bool ignore, IDictionary<string, string> queryParameters)
            : this()
        {
            ID = id;
            PageID = pageID;
            Path = path;
            TemplateUrl = templateUrl;
            Action = action;
            Argument = arguments;
            Ignore = ignore;
            QueryParameters = new Dictionary<string, string>(queryParameters);
        }

        public PathData()
        {
            QueryParameters = new Dictionary<string, string>();
            IsRewritable = true;
            IsCacheable = true;
        }

        /// <summary>The item behind this path.</summary>
        public ContentItem CurrentItem 
        {
            get { return Get(ref currentItem, ID); }
            set { ID = Set(ref currentItem, value); }
        }

        /// <summary>The page behind this path (might differ from CurrentItem when the path leads to a part).</summary>
        public ContentItem CurrentPage
        {
            get { return Get(ref currentPage, PageID) ?? CurrentItem; }
            set { PageID = Set(ref currentPage, value); }
        }

        /// <summary>The item reporting that the path isn't a match.</summary>
        public ContentItem StopItem
        {
            get { return Get(ref stopItem, StopID); }
            set { StopID = Set(ref stopItem, value); }
        }

        /// <summary>The template handling this path.</summary>
        public string TemplateUrl { get; set; }
        
        /// <summary>The identifier of the content item behind this path.</summary>
        public int ID { get; set; }

        /// <summary>The identifier of the content page behind this path.</summary>
        public int PageID { get; set; }

        /// <summary>The identifier of the content page this path data originates from.</summary>
        public int StopID { get; set; }

        /// <summary>?</summary>
        public string Path { get; set; }

        /// <summary>An optional controller info used by MVC routing.</summary>
        public string Controller { get; set; }

        /// <summary>An optional action to separate templates handling an item.</summary>
        public string Action { get; set; }

        /// <summary>An additional argument to the action.</summary>
        public string Argument { get; set; }

        /// <summary>Query parameters passed on to the template.</summary>
        public IDictionary<string, string> QueryParameters { get; set; }
        
        /// <summary>Indicates that an existing file handle this path and it shouldn't be rewritten.</summary>
        public bool Ignore { get; set; }
        
        /// <summary>Indicates that this path may be rewritten.</summary>
        public bool IsRewritable { get; set; }
        
        /// <summary>Indicates that this path may be cached.</summary>
        public bool IsCacheable { get; set; }

        /// <summary>Read permissions allow everyone to read this path. Not altering read permissions allow the system to make certain optimizations.</summary>
        public bool IsPubliclyAvailable { get; set; }

        [Obsolete("Use path.GetRewrittenUrl() extension method available when using N2.Web namespace")]
        public virtual Url RewrittenUrl
        {
            get
            {
                return this.GetRewrittenUrl();
            }
        }

        public virtual PathData UpdateParameters(IDictionary<string, string> queryString)
        {
            foreach (KeyValuePair<string, string> pair in queryString)
            {
                if (string.Equals(pair.Key, "argument"))
                    Argument = pair.Value;
                else
                    QueryParameters[pair.Key] = pair.Value;
            }

            return this;
        }

        /// <summary>Returns a copy of the path data stripped of the request specific instance of the content item.</summary>
        /// <returns>A copy of the path data.</returns>
        public virtual PathData Detach()
        {
            PathData data = Clone();

            // clear persistent objects before caching
            data.currentItem = null;
            data.currentPage = null;
            data.stopItem = null;
            data.persister = null;
            return data;
        }

        /// <summary>Creates a copy of the PathData with a content item retrieved from the supplied persisterToAttach. The reason for this is that PathData can be cached and we don't want to share instances between requests.</summary>
        /// <param name="persisterToAttach">The perister providing the item.</param>
        /// <returns>A copy of the path data.</returns>
        public virtual PathData Attach(IPersister persisterToAttach)
        {
            PathData data = Clone();

            // the persisterToAttach is used to lazily load persistent CurrentItem/CurrentPage/StopItem
            data.persister = persisterToAttach;

            return data;
        }

        public virtual PathData SetArguments(string argument)
        {
            Argument = argument;
            return this;
        }

        /// <summary>Checks whether the path contains data.</summary>
        /// <returns>True if the path is empty.</returns>
        public virtual bool IsEmpty()
        {
            return currentItem == null 
                && (persister == null || ID == 0);
        }

        private ContentItem Get(ref ContentItem item, int id)
        {
            if (item != null)
                return item;
            if (persister != null && id != 0)
                return item = persister.Get(id);
            return null;
        }

        private int Set(ref ContentItem current, ContentItem value)
        {
	        current = value;
            OnItemChange(current, value);
            return value != null ? value.ID : 0;
        }

        protected virtual void OnItemChange(ContentItem current, ContentItem value)
        {
			/*
			 * dead code ... [Obsolete]
			 * 
            var newPermission = Security.Permission.None;
            if (currentItem != null)
                newPermission |= currentItem.AlteredPermissions;
            if (currentPage != null)
                newPermission |= currentPage.AlteredPermissions;
			*/

            IsPubliclyAvailable = IsPubliclyAvailableOrEmpty(currentItem) && IsPubliclyAvailableOrEmpty(currentPage);
        }

        private bool IsPubliclyAvailableOrEmpty(ContentItem item)
        {
            return item == null
                || ((item.State == ContentState.Published || item.State == ContentState.New || item.State == ContentState.None)
                && (item.AlteredPermissions & Security.Permission.Read) == Security.Permission.None);
        }

        /// <summary>Clones the path data instance so that modifications doesn't affect the caller.</summary>
        /// <returns>A clone of the path data object.</returns>
        public virtual PathData Clone()
        {
            var clone = (PathData)MemberwiseClone();
	        clone.QueryParameters = new Dictionary<string, string>(QueryParameters);
            return clone;
        }

        /// <summary>Clones the path data instance so that modifications doesn't affect the caller.</summary>
        /// <returns>A clone of the path data object.</returns>
        public virtual PathData Clone(ContentItem page, ContentItem item)
        {
            var clone = Clone();
            clone.CurrentItem = item;
            clone.CurrentPage = page;
            return clone;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public override string ToString()
        {
	        if (PageID != 0 && PageID != ID)
                return PageID + "/" + ID;
	        return ID == 0 ? string.Empty : ID.ToString(CultureInfo.InvariantCulture);
        }

	    /// <summary>Parses a string representation of this pathdata giving a detached path data object.</summary>
        /// <param name="pathString">A string created via <see cref="ToString"/>.</param>
        /// <returns>A path with the given values or an empty path data.</returns>
        public static PathData Parse(string pathString)
        {
            if (string.IsNullOrEmpty(pathString))
                return Empty;

            var parts = pathString.Split('/');
            if (parts.Length > 1)
            {
                int id, pageId;
                if (int.TryParse(parts[0], out pageId) && int.TryParse(parts[1], out id))
                    return new PathData { ID = id, PageID = pageId };
            }
            else
            {
                int id;
                if (int.TryParse(pathString, out id))
                    return new PathData { ID = id };
            }

            return Empty;
        }

        /// <summary>Parses a string representation of this pathdata and reconnects the items to the session.</summary>
        /// <param name="pathString">A string created via <see cref="ToString"/>.</param>
        /// <param name="persister">A persisterToAttach that will be used to get the items referenced by the path string.</param>
        /// <returns>A path with the given values or an empty path data.</returns>
        public static PathData Parse(string pathString, IPersister persister)
        {
            var path = Parse(pathString);
            return path.Attach(persister);
        }
    }
}
