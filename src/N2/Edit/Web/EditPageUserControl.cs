using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using N2.Engine;

namespace N2.Edit.Web
{
	public class EditPageUserControl : UserControl
	{
		public new EditPage EditPage
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
