using System;
using System.Web.UI;
using N2.Engine;

namespace N2.Edit.Web
{
    /// <summary>
    /// A user control that provides access to the edit item page.
    /// </summary>
    public class EditPageUserControl : UserControl
    {
        [Obsolete]
        public EditPage EditPage
        {
            get { return base.Page as EditPage; }
        }

        public IEngine Engine
        {
            get { return (base.Page as EditPage) != null ? (base.Page as EditPage).Engine : N2.Context.Current; }
        }

        SelectionUtility selection;
        protected SelectionUtility Selection
        {
            get { return selection ?? (selection = new SelectionUtility(this, Engine)); }
            set { selection = value; }
        }

        [Obsolete]
        public ContentItem SelectedItem
        {
            get { return Selection.SelectedItem; }
        }
    }
}
