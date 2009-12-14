using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using N2.Web;
using N2.Engine;

namespace N2.Edit
{
    /// <summary>
    /// Helps discovering item selection in editing interfaces.
    /// </summary>
    public class SelectionUtility
    {
        Control container;
        IEngine engine;
        ContentItem selectedItem;
        ContentItem memorizedItem;

        public SelectionUtility(Control container, IEngine engine)
        {
            this.container = container;
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
            return engine.Resolve<Navigator>().Navigate(container.Page.Request["memory"]);
        }

        private ContentItem GetSelectionFromUrl()
        {
            string selected = container.Page.Request["selected"];
            if (!string.IsNullOrEmpty(selected))
                return engine.Resolve<Navigator>().Navigate(selected);

            string selectedUrl = container.Page.Request["selectedUrl"];
            if (!string.IsNullOrEmpty(selectedUrl))
                return engine.UrlParser.Parse(selectedUrl);

            string itemId = container.Page.Request[PathData.ItemQueryKey];
            if (!string.IsNullOrEmpty(itemId))
                return engine.Persister.Get(int.Parse(itemId));

            return null;
        }
    }
}
