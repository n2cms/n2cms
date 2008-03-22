using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

namespace N2.Edit
{
    public partial class Default : Web.EditPage
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			try
			{
				// These fields are used client side to store selected items
				Page.ClientScript.RegisterHiddenField("selected", SelectedItem.Path);
				Page.ClientScript.RegisterHiddenField("memory", "");
				Page.ClientScript.RegisterHiddenField("action", "");
			}
			catch(NullReferenceException ex)
			{
				string url = GetSelectedPath();
				if(url == null)
					throw  new N2Exception("Couldn't get the start page, this usually indicates a configuration or installation problem. The start page must be inserted and it's id must be configured in web.config.", ex);
				else 
					Response.Write("Error: Couldn't find '" + url + "'.");
			}
		}
	}
}