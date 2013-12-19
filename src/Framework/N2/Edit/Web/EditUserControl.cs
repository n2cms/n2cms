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
            get { return selection ?? (selection = (Page is EditPage) ? (Page as EditPage).Selection : new SelectionUtility(this, Engine)); }
            set { selection = value; }
        }

        [Obsolete("Use Selection.SelectedItem")]
        protected virtual N2.ContentItem SelectedItem
        {
            get { return Selection.SelectedItem; }
        }

        protected virtual void Refresh(ContentItem item)
        {
            Page.RefreshManagementInterface(item);
        }

        protected virtual void Refresh(ContentItem item, string previewUrl)
        {
            Page.RefreshPreviewFrame(item, previewUrl);
        }

        /// <summary>Referesh the selected frames after loading the page.</summary>
        /// <param name="item"></param>
        /// <param name="area"></param>
        protected virtual void Refresh(ContentItem item, ToolbarArea area, bool force = true)
        {
            Page.RefreshFrames(item, area, force);
        }

        protected virtual string GetLocalResourceString(string resourceKey, string defaultText)
        {
            try
            {
                return GetLocalResourceObject(resourceKey) as string ?? defaultText;
            }
            catch (Exception)
            {
                return defaultText;
            }
        }
    }
}
