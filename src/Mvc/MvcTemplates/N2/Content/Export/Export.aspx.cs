using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Persistence.Serialization;
using N2.Edit.Web;
using N2.Edit;
using N2.Definitions;

namespace N2.Management.Content.Export
{
    public partial class Export : EditPage
    {
        protected AffectedItems exportedItems;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            tpImport.NavigateUrl = "Default.aspx?selected=" + Selection.SelectedItem.Path;

            if (Selection.SelectedItem is IFileSystemNode)
            {
                Response.Redirect("../../Files/FileSystem/Export.aspx?path=" + Server.UrlEncode(Selection.SelectedItem.Path) + "#ctl00_ctl00_Frame_Content_tpExport");
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            exportedItems.CurrentItem = Selection.SelectedItem;
            tpExport.DataBind();
        }

        protected void btnExport_Command(object sender, CommandEventArgs e)
        {
            ExportOptions options = ExportOptions.Default;
            if (chkDefinedDetails.Checked)
                options |= ExportOptions.OnlyDefinedDetails;
            if (chkAttachments.Checked)
                options |= ExportOptions.ExcludeAttachments;

            Engine.Resolve<Exporter>().Export(Selection.SelectedItem, options, Response);
        }
    }
}
