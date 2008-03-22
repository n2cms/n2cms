using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Web.UI
{
	public class ItemDataSourceSelectingEventArgs : EventArgs
	{
		public ItemDataSourceSelectingEventArgs(DataSourceSelectArguments arguments)
		{
			this.arguments = arguments;
		}
		private DataSourceSelectArguments arguments;
		public DataSourceSelectArguments Arguments
		{
			get { return arguments; }
		}
	}
}
