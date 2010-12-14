using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using N2.Edit.Web;

namespace N2.Edit.Globalization
{
	public partial class Languages : EditPageUserControl
	{
		private object dataSource;

		public object DataSource
		{
			get { return dataSource; }
			set { dataSource = value; }
		}

		protected string GetClass()
		{
			string className = (bool)Eval("IsNew") ? "new" : "existing";

			if (Selection.SelectedItem == (ContentItem)Eval("ExistingItem"))
				className += " current";

			return className;
		}
	}
}