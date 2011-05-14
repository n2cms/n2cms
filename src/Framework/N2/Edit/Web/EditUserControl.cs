using System;
using System.Web.UI;
using N2.Engine;

namespace N2.Edit.Web
{
    /// <summary>
    /// Base class for user controls in the edit interface.
    /// </summary>
    public abstract class EditUserControl : UserControl
    {
        IEngine engine;
        /// <summary>The default engine driving this application.</summary>
        public IEngine Engine
        {
            get { return engine ?? (engine = N2.Context.Current); }
            set { engine = value; }
        }

        SelectionUtility selection;
        /// <summary>Helps to select items from query string.</summary>
        protected SelectionUtility Selection
        {
            get { return selection ?? (selection = new SelectionUtility(this, Engine)); }
            set { selection = value; }
        }

    	[Obsolete("Use Selection.SelectedItem")]
        protected virtual N2.ContentItem SelectedItem
        {
            get { return Selection.SelectedItem; }
        }
    }
}
