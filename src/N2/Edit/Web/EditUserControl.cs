using System.Web.UI;
using N2.Engine;

namespace N2.Edit.Web
{
    /// <summary>
    /// Base class for user controls in the edit interface.
    /// </summary>
    public abstract class EditUserControl : UserControl
    {
        protected IEngine Engine
        {
            get { return N2.Context.Current; }
        }

    	N2.ContentItem selectedItem;
        protected virtual N2.ContentItem SelectedItem
        {
            get
            {
                string itemId = Request.QueryString["item"];
                string selected = Request.QueryString["selected"];
				if (selectedItem != null)
					return selectedItem;

				if (!string.IsNullOrEmpty(selected))
                    selectedItem = Engine.UrlParser.Parse(selected);
                if (!string.IsNullOrEmpty(itemId))
                    selectedItem= Engine.Persister.Get(int.Parse(itemId));
                else
                    selectedItem = Engine.UrlParser.StartPage;

            	return selectedItem;
            }
        }
    }
}
