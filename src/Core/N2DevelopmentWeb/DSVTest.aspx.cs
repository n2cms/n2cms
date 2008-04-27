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
using N2;

namespace N2DevelopmentWeb
{
	public partial class DSVTest : N2.Web.UI.Page<Domain.AbstractCustomItem>
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
				this.gvChildren.SelectedIndex = 0;
		}

		protected void gvChildren_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.dvItem.PageIndex = this.gvChildren.SelectedIndex;
		}

		protected void dvItem_PageIndexChanged(object sender, EventArgs e)
		{
			this.gvChildren.SelectedIndex = this.dvItem.PageIndex;
		}

		protected void ddlPaths_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.ids.Path = ((DropDownList)sender).SelectedValue;
			this.DataBind();
		}

		protected void ids_DataSourceChanged(object sender, EventArgs e)
		{
			Response.Write("DataSourceChanged");
		}

		protected void ids_Deleted(object sender, N2.Collections.ItemListEventArgs e)
		{
			Report("Deleted", sender, e);
		}

		protected void ids_Deleting(object sender, N2.Collections.ItemListEventArgs e)
		{
			Report("Deleting", sender, e);
		}

		protected void ids_Filtering(object sender, N2.Collections.ItemListEventArgs e)
		{
			Report("Filtering", sender, e);
		}

		protected void ids_Inserted(object sender, N2.Collections.ItemListEventArgs e)
		{
			Report("Inserted", sender, e);
		}

		protected void ids_Inserting(object sender, N2.Collections.ItemListEventArgs e)
		{
			Report("Inserting", sender, e);
		}

		protected void ids_ItemCreated(object sender, N2.Persistence.ItemEventArgs e)
		{
			Report("ItemCreated", sender, e);
		}

		protected void ids_ItemCreating(object sender, N2.Persistence.ItemEventArgs e)
		{
			Report("ItemCreating", sender, e);
		}

		protected void ids_Selected(object sender, N2.Collections.ItemListEventArgs e)
		{
			Report("Selected", sender, e);
		}

		protected void ids_Selecting(object sender, N2.Web.UI.ItemDataSourceSelectingEventArgs e)
		{
			Report("Selecting", sender, e);
		}

		protected void ids_Updated(object sender, N2.Collections.ItemListEventArgs e)
		{
			Report("Updated", sender, e);
		}

		protected void ids_Updating(object sender, N2.Collections.ItemListEventArgs e)
		{
			Report("Updating", sender, e);
		}

		private void Report(string action, object sender, N2.Collections.ItemListEventArgs e)
		{
			Response.Write("<li>" + sender.GetType().Name + ", <b>" + action + "</b>: ");
			foreach (ContentItem item in e.Items)
				Response.Write(item.Name + ", ");
		}
		private void Report(string action, object sender, N2.Persistence.ItemEventArgs e)
		{
			Response.Write("<li>" + sender.GetType().Name + ", <b>" + action + "</b>: ");
			Response.Write(e.AffectedItem.Name + ", ");
		}
		private void Report(string action, object sender, N2.Web.UI.ItemDataSourceSelectingEventArgs e)
		{
			Response.Write("<li>" + sender.GetType().Name + ", <b>" + action + "</b>: ");
			Response.Write(e.Arguments.SortExpression + ", ");
		}
	}
}
