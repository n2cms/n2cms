using System.Web;
using System.Web.UI;
using N2.Engine;
using N2.Web;

namespace N2.Edit
{
    /// <summary>
    /// Helps discovering item selection in editing interfaces.
    /// </summary>
    public class SelectionUtility
    {
        HttpRequest request;
		IEngine engine;
        ContentItem selectedItem;
        ContentItem memorizedItem;

		protected IEngine Engine
		{
			get { return engine ?? (engine = N2.Context.Current); }
			set { engine = value; }
		}

        public SelectionUtility(Control container, IEngine engine)
        {
			this.request = container.Page.Request;
            this.Engine = engine;
		}

		public SelectionUtility(HttpRequest request, IEngine engine)
		{
			this.request = request;
			this.Engine = engine;
		}

		public SelectionUtility(ContentItem selectedItem, ContentItem memorizedItem)
		{
			this.selectedItem = selectedItem;
			this.memorizedItem = memorizedItem;
		}

        /// <summary>The selected item.</summary>
        public ContentItem SelectedItem
        {
            get { return selectedItem ?? (selectedItem = GetSelectionFromUrl() ?? Engine.UrlParser.StartPage); }
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
			if (request == null) return null; // explicitly passed memory

            return Engine.Resolve<Navigator>().Navigate(request["memory"]);
        }

        private ContentItem GetSelectionFromUrl()
        {
			if (request == null) return null; // explicitly passed selection

			string selected = request[SelectedQueryKey];
            if (!string.IsNullOrEmpty(selected))
                return Engine.Resolve<Navigator>().Navigate(HttpUtility.UrlDecode(selected));

            string selectedUrl = request["selectedUrl"];
			if (!string.IsNullOrEmpty(selectedUrl))
				return Engine.UrlParser.Parse(selectedUrl)
					?? SelectFile(selectedUrl);

            string itemId = request[PathData.ItemQueryKey];
            if (!string.IsNullOrEmpty(itemId))
                return Engine.Persister.Get(int.Parse(itemId));

            return null;
        }

		private ContentItem SelectFile(string selectedUrl)
		{
			string location = request.QueryString["location"];
			if (string.IsNullOrEmpty(location))
				return null;
			if(Url.Parse(selectedUrl).IsAbsolute)
				return null;

			string url = Url.ToRelative(selectedUrl).TrimStart('~');
			if (!url.StartsWith("/"))
				return null; ;

			return Engine.Resolve<Navigator>().Navigate(url);
		}

		public string ActionUrl(string actionName)
		{
			return Url.Parse(SelectedItem.FindPath(actionName).TemplateUrl).AppendQuery(SelectedQueryKey, SelectedItem.Path).ResolveTokens();
		}


		static SelectionUtility()
		{
			SelectedQueryKey = "selected";
		}
		public static string SelectedQueryKey { get; set; }
	}
}
