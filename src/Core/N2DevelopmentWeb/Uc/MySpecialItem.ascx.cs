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

namespace N2DevelopmentWeb.Uc
{
    public partial class MySpecialItem 
        : N2.Web.UI.UserControl<N2.ContentItem, N2DevelopmentWeb.Domain.MyItemSubData>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                this.DataBind();
        }

		protected void btnPostback_Click(object sender, EventArgs e)
		{
			int count = int.Parse(this.label2.Text);
			count++;
			this.label2.Text = count.ToString();
		}
    }
}