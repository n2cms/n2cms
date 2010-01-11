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
			rptCns.DataSource = ConfigurationManager.ConnectionStrings;
			rptCns.DataBind();

			string connectionStringName = Request.QueryString["cn"] ?? ConfigurationManager.ConnectionStrings[0].Name;
			sdsItems.ConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

			if (!IsPostBack)
			{
				LoadItems();
			}
			base.OnInit(e);
		}

		private void LoadItems()
		{
			gvItems.DataSourceID = sdsItems.ID;
			try
			{
				lblError.Text = "";
				gvItems.DataBind();
			}
			catch (Exception ex)
			{
				gvItems.DataSourceID = "";
				lblError.Text = ex.Message;
			}
		}
	}
}
