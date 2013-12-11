using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Addons.UITests.UI
{
    public partial class GridViewTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void gvChildren_PageIndexChanged(object sender, EventArgs e)
        {

        }

        protected void gvChildren_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

        }

        protected void gvChildren_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {

        }

        protected void gvChildren_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            Response.Write("gvChildren_RowCommand " + e.CommandSource + " " + e.CommandName + " " + e.CommandArgument);
        }

        protected void gvChildren_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {

        }

        protected void gvChildren_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void gvChildren_RowEditing(object sender, GridViewEditEventArgs e)
        {

        }

        protected void gvChildren_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {

        }

        protected void gvChildren_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }

        protected void gvChildren_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void gvChildren_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {

        }

        protected void gvChildren_Sorted(object sender, EventArgs e)
        {

        }

        protected void gvChildren_Sorting(object sender, GridViewSortEventArgs e)
        {

        }

        protected void gvChildren_RowCreated(object sender, GridViewRowEventArgs e)
        {

        }

        protected void gvChildren_DataBinding(object sender, EventArgs e)
        {

        }

        protected void gvChildren_DataBound(object sender, EventArgs e)
        {

        }

        protected void gvChildren_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }
    }
}
