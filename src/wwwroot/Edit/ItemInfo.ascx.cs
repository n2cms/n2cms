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
	public partial class ItemInfo : System.Web.UI.UserControl
	{
		protected Definitions.ItemDefinition CurrentDefinition
		{
			get { return N2.Context.Definitions.GetDefinition(CurrentItem.GetType()); }
		}

		public ContentItem currentItem;
		public ContentItem CurrentItem
		{
			get { return currentItem; }
			set { currentItem = value; }
		}
	}
}