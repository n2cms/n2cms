using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using N2.Engine;

namespace N2.Edit.Web
{
	public class EditUserControl : UserControl
	{
		public new EditPage Page
		{
			get
			{
				return base.Page as EditPage;
			}
		}

		public IEngine Engine
		{
			get
			{
				return this.Page.Engine;
			}
		}

		public ContentItem SelectedItem
		{
			get
			{
				return this.Page.SelectedItem;
			}
		}
	}
}
