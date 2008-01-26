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

namespace N2.TemplateWeb.Templates
{
	public partial class LinkedDetail : N2.Web.UI.Page<Domain.MyItemWithLinks>
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			this.DataBind();

			foreach (object o in CurrentItem.Details)
				Response.Write("this: " + o != null);
		}

		protected void Button1_Click(object sender, EventArgs e)
		{
			Details.DetailCollection dc = CurrentItem.GetDetailCollection("relatedList", true);
			dc.AddRange(CurrentItem.Parent.Children);
			this.DataBind();
		}
		protected void Button2_Click(object sender, EventArgs e)
		{
			Details.DetailCollection dc = CurrentItem.GetDetailCollection("relatedList2", true);
			dc.AddRange(CurrentItem.Parent.Children);
			this.DataBind();
		}
	}
}
