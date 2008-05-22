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
using System.Collections.Generic;

namespace N2.Templates.Search.UI
{
	public partial class Default : Web.UI.TemplatePage<Items.AbstractSearch>
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			Resources.Register.StyleSheet(this, "~/Search/UI/Css/Search.css", N2.Resources.Media.All);
		}

		private ICollection<ContentItem> hits = new List<ContentItem>();

		protected ICollection<ContentItem> Hits
		{
			get { return hits; }
			set { hits = value; }
		}

		protected void btnSearch_Click(object sender, EventArgs e)
		{
			Hits = CurrentPage.Search(txtQuery.Text);
			
			DataBind();
		}
	}
}
