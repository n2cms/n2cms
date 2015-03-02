using System.Web;
using System.Web.UI;
using N2.Engine;
using N2.Web;
using N2.Edit.Versioning;
using N2.Collections;
using System;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.IO;

namespace N2.Edit
{
    /// <summary>
    /// Helps discovering item selection in editing interfaces.
    /// </summary>
    public class SelectionUtility
    {
        Func<string, string> requestValueAccessor = (key) => null;
        IEngine engine;
        ContentItem selectedItem;
        ContentItem memorizedItem;

        protected IEngine Engine
        {
            get { return engine ?? (engine = N2.Context.Current); }
            set { engine = value; }
        }

        public Func<string, string> RequestValueAccessor
        {
            get { return requestValueAccessor; }
            set { requestValueAccessor = value; }
        }

        public SelectionUtility(Control container, IEngine engine)
        {
            this.RequestValueAccessor = (key) => container.Page.Request[key];
            this.Engine = engine;
        }

        public SelectionUtility(HttpContext context, IEngine engine)
            : this(context.GetHttpContextBase(), engine)
        {
        }

        public SelectionUtility(HttpContextBase context, IEngine engine)
        {
            this.RequestValueAccessor = context.GetRequestValueAccessor();
            this.Engine = engine;
        }

        public SelectionUtility(Func<string, string> accessor, IEngine engine)
        {
            this.RequestValueAccessor = accessor;
            this.engine = engine;
        }

        public SelectionUtility(ContentItem selectedItem, ContentItem memorizedItem)
        {;
            this.selectedItem = selectedItem;
            this.memorizedItem = memorizedItem;
        }

        public TraverseHelper Traverse
        {
            get { return new TraverseHelper(() => Engine, new FilterHelper(() => Engine), () => new PathData(SelectedItem)); }
        }

        /// <summary>The selected item.</summary>
        public ContentItem SelectedItem
        {
            get { return selectedItem ?? (selectedItem = ParseSelectionFromRequest() ?? Engine.UrlParser.StartPage); }
            set { selectedItem = value; }
        }

        /// <summary>The item placed in memory.</summary>
        public ContentItem MemorizedItem
        {
            get { return memorizedItem ?? (memorizedItem = GetMemoryFromUrl()); }
            set { memorizedItem = value; }
        }

        private ContentItem GetMemoryFromUrl()
        {
            if (RequestValueAccessor == null) return null; // explicitly passed memory

            return Engine.Resolve<Navigator>().Navigate(RequestValueAccessor("memory"));
        }

        /// <summary>Analyzes the request trying to determine the selected item.</summary>
        /// <returns>A content item or null if no item was discovered.</returns>
        public virtual ContentItem ParseSelectionFromRequest()
        {
            if (RequestValueAccessor == null) return null; // explicitly passed selection

            var selectedItem = ParseSelected(RequestValueAccessor(SelectedQueryKey));

            var selectedUrl = RequestValueAccessor("selectedUrl");
            if (!string.IsNullOrEmpty(selectedUrl))
                selectedItem = ParseUrl(selectedUrl);

            string itemId = RequestValueAccessor(PathData.ItemQueryKey);
            if (!string.IsNullOrEmpty(itemId))
                selectedItem = Engine.Persister.Get(int.Parse(itemId)) ?? selectedItem;
            
            if (selectedItem != null && RequestValueAccessor(PathData.VersionIndexQueryKey) != null)
            {
                selectedItem = ParseSpecificVersion(selectedItem) ?? selectedItem;
            }
            else if (selectedItem != null && RequestValueAccessor(WebExtensions.ViewPreferenceQueryString) != null)
            {
                selectedItem = ParseLatestDraft(selectedItem) ?? selectedItem;
            }
            return selectedItem;
        }

        private ContentItem ParseLatestDraft(ContentItem selectedItem)
        {
            var dr = Engine.Resolve<DraftRepository>();
            if (dr.HasDraft(selectedItem))
            {
                var draft = dr.GetDraftInfo(selectedItem);
                var cvr = Engine.Resolve<ContentVersionRepository>();
                selectedItem = cvr.ParseVersion(draft.VersionIndex.ToString(), RequestValueAccessor(PathData.VersionKeyQueryKey), selectedItem);
            }
            return selectedItem;
        }

        private ContentItem ParseSpecificVersion(ContentItem selectedItem)
        {
            var cvr = Engine.Resolve<ContentVersionRepository>();
            selectedItem = cvr.ParseVersion(RequestValueAccessor(PathData.VersionIndexQueryKey), RequestValueAccessor(PathData.VersionKeyQueryKey), selectedItem);
            return selectedItem;
        }

        public ContentItem ParseSelected(string selected)
        {
            if (string.IsNullOrEmpty(selected))
                return null;
            
            int id;
            if (int.TryParse(selected, out id))
                return engine.Persister.Get(id);

            var selectedItem = Engine.Resolve<Navigator>().Navigate(HttpUtility.UrlDecode(selected));
            return selectedItem;
        }

        public ContentItem ParseUrl(Url selectedUrl)
        {
            if (string.IsNullOrEmpty(selectedUrl))
                return null;

            var selectedItem = Engine.UrlParser.Parse(selectedUrl);
            int versionIndex;
            if (selectedItem == null)
                selectedItem = ParseSelected(selectedUrl[PathData.SelectedQueryKey]);

            if (selectedItem == null)
                return SelectFile(selectedUrl);
            else if (int.TryParse(selectedUrl[PathData.VersionIndexQueryKey], out versionIndex))
                return Engine.Resolve<IVersionManager>().GetVersion(selectedItem, versionIndex);
            else if (string.Equals(selectedUrl[WebExtensions.ViewPreferenceQueryString], WebExtensions.DraftQueryValue, StringComparison.InvariantCultureIgnoreCase))
                return TryApplyDraft(selectedItem);

            return selectedItem;
        }

        private ContentItem TryApplyDraft(ContentItem selectedItem)
        {
            var drafts = Engine.Resolve<DraftRepository>();
            if (drafts.HasDraft(selectedItem))
            {
                var version = drafts.Versions.GetVersion(selectedItem);
                if (version != null)
				{
					var item = drafts.Versions.DeserializeVersion(version);
					if (item != null)
						selectedItem = item;
				}
            }
            return selectedItem;
        }

        private ContentItem SelectFile(string selectedUrl)
        {
            string location = RequestValueAccessor("location");
            if (string.IsNullOrEmpty(location))
                return null;
            if (Url.Parse(selectedUrl).IsAbsolute)
            {
                foreach (var folder in Engine.Resolve<UploadFolderSource>().GetUploadFoldersForCurrentSite())
                    if (!string.IsNullOrEmpty(folder.UrlPrefix) && selectedUrl.StartsWith(folder.UrlPrefix, System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        var source = Engine.Resolve<N2.Persistence.Sources.ContentSource>();
                        var path = source.ResolvePath(selectedUrl.Substring(folder.UrlPrefix.Length));
                        return path.CurrentItem;
                    }
                return null;
            }

            string url = Url.ToRelative(selectedUrl).TrimStart('~');
            if (!url.StartsWith("/"))
                return null; ;

            return Engine.Resolve<Navigator>().Navigate(url);
        }

        public string ActionUrl(string actionName)
        {
            return Url.Parse(SelectedItem.FindPath(actionName).TemplateUrl).AppendQuery(SelectedQueryKey, SelectedItem.Path).ResolveTokens();
        }

        public string SelectedUrl(Url baseUrl, ContentItem selected = null)
        {
            return baseUrl.AppendQuery(SelectedQueryKey, (selected ?? SelectedItem).Path).ResolveTokens();
        }


        static SelectionUtility()
        {
            SelectedQueryKey = "selected";
        }

        public static string SelectedQueryKey { get; set; }
    }
}
