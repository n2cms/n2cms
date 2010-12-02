using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using N2.Web;
using N2.Engine;
using System.Web;

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

        public SelectionUtility(Control container, IEngine engine)
        {
			this.request = container.Page.Request;
            this.engine = engine;
		}

		public SelectionUtility(HttpRequest request, IEngine engine)
		{
			this.request = request;
			this.engine = engine;
		}

        /// <summary>The selected item.</summary>
        public ContentItem SelectedItem
        {
            get { return selectedItem ?? (selectedItem = GetSelectionFromUrl() ?? engine.UrlParser.StartPage); }
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
            return engine.Resolve<Navigator>().Navigate(request["memory"]);
        }

        private ContentItem GetSelectionFromUrl()
        {
            string selected = request["selected"];
            if (!string.IsNullOrEmpty(selected))
                return engine.Resolve<Navigator>().Navigate(HttpUtility.UrlDecode(selected));

            string selectedUrl = request["selectedUrl"];
			if (!string.IsNullOrEmpty(selectedUrl))
				return engine.UrlParser.Parse(selectedUrl)
					?? SelectFile(selectedUrl);

            string itemId = request[PathData.ItemQueryKey];
            if (!string.IsNullOrEmpty(itemId))
                return engine.Persister.Get(int.Parse(itemId));

            return null;
        }

		private ContentItem SelectFile(string selectedUrl)
		{
			string location = request.QueryString["location"];
			if (string.IsNullOrEmpty(location))
				return null;

			return engine.Resolve<Navigator>().Navigate(Url.ToRelative(selectedUrl).TrimStart('~'));
		}

		public string ActionUrl(string actionName)
		{
			return Url.Parse(SelectedItem.FindPath(actionName).TemplateUrl).AppendQuery("selected", SelectedItem.Path).ResolveTokens();
		}
	}
}
