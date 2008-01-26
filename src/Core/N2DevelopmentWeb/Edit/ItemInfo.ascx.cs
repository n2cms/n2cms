using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace N2.Edit
{
	public partial class ItemInfo : System.Web.UI.UserControl, N2.Web.UI.IDataItemContainer
	{
		protected Definitions.ItemDefinition CurrentDefinition
		{
			get { return N2.Context.Definitions.GetDefinition(CurrentItem.GetType()); }
		}

		#region IDataItemContainer Members

		public ContentItem currentData;
		public ContentItem CurrentData
		{
			get { return currentData; }
			set { currentData = value; }
		}

		#endregion

		#region IItemContainer Members

		public ContentItem CurrentItem
		{
			get { return CurrentData; }
		}

		#endregion
	}
}