using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace N2.Edit.Web.UI.Controls
{
	public class InfoLabel : Label
	{
		public string Label
		{
			get { return (string)(ViewState["Label"] ?? ""); }
			set { ViewState["Label"] = value; }
		}

		public override void RenderBeginTag(HtmlTextWriter writer)
		{
			if (this.Label.Length > 0 && this.Text.Length > 0)
			{
				writer.Write("<label for='");
				writer.Write(this.ClientID);
				writer.Write("'>");
				writer.Write(this.Label);
				writer.Write("</label>");
			}
			base.RenderBeginTag(writer);
		}
	}
}
