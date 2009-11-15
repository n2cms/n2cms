using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using N2.Engine;
using N2.Web;

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
			get { return this.EditPage.Engine; }
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

        [Obsolete]
        public HtmlHelper Html
        {
            get { return new HtmlHelper(this.Page, SelectedItem); }
        }
	}
}
