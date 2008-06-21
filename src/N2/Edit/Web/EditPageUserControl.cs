using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using N2.Engine;

namespace N2.Edit.Web
{
    /// <summary>
    /// A user control that provides access to the edit item page.
    /// </summary>
	public class EditPageUserControl : UserControl
	{
		public EditPage EditPage
		{
			get { return base.Page as EditPage; }
		}

		public IEngine Engine
		{
			get
			{
				return this.EditPage.Engine;
			}
		}

		public ContentItem SelectedItem
		{
			get
			{
				return this.EditPage.SelectedItem;
			}
		}
	}
}
