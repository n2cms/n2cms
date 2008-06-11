using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
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

		protected N2.ContentItem SelectedItem
		{
			get
			{
				string itemId = Request.QueryString["item"];
				string selected = Request.QueryString["selected"];
				if (!string.IsNullOrEmpty(selected))
					return Engine.UrlParser.Parse(selected);
				if (!string.IsNullOrEmpty(itemId))
                    return Engine.Persister.Get(int.Parse(itemId));
				else
                    return Engine.UrlParser.StartPage;
			}
		}
	}
}
