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

namespace N2.Edit.Install
{
	public partial class Fix : System.Web.UI.Page
	{
		protected override void OnInit(EventArgs e)
		{
			ddlCNs.DataSource = ConfigurationManager.ConnectionStrings;
			ddlCNs.DataBind();
			ddlCNs.SelectedIndexChanged += new EventHandler(ddlCNs_SelectedIndexChanged);
			sdsItems.ConnectionString = ddlCNs.SelectedValue;
			base.OnInit(e);
		}

		void ddlCNs_SelectedIndexChanged(object sender, EventArgs e)
		{
			sdsItems.ConnectionString = ddlCNs.SelectedValue;
		}
	}
}
