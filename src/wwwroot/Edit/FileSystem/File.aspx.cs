using System;
using System.Web.UI.WebControls;
using N2.Edit.Web;
using N2.Edit.FileSystem.Items;

namespace N2.Edit.FileSystem
{
    public partial class File1 : EditPage
    {
        protected File SelectedFile
        {
            get { return SelectedItem as File; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Title = SelectedItem.Title;
        }

    	protected void OnDownloadCommand(object sender, CommandEventArgs e)
    	{
			Response.ContentType = "application/octet-stream";
			Response.AppendHeader("Content-disposition", "attachment; filename=" + SelectedFile.Name);
			Response.TransmitFile(SelectedFile.PhysicalPath);
			Response.End();
    	}
    }
}
