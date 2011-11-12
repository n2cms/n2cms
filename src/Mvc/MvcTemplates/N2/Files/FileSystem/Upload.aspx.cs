using System;
using System.Configuration;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using N2.Edit.FileSystem;
using N2.Edit.Web;
using N2.Resources;

namespace N2.Management.Files.FileSystem
{
	public partial class Upload : EditPage
	{
		protected int maxFileSize = 4096 * 1024 - 10000;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Page.JQueryUi();
        }

		protected void Page_Load(object sender, EventArgs e)
		{
			hlCancel.NavigateUrl = Selection.ActionUrl("info");
            
			try
			{
				var config = ConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection;
				maxFileSize = config.MaxRequestLength * 1024 - 10000;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex);
			}
		}

        //protected void btnAlternative_Command(object sender, CommandEventArgs args)
        //{
        //    if(fuAlternative.PostedFile.ContentLength > 0)
        //    {
        //        string filename = System.IO.Path.GetFileName(fuAlternative.PostedFile.FileName);
        //        string url = Selection.SelectedItem.Url.TrimEnd('/') + "/" + filename;
        //        Engine.Resolve<IFileSystem>().WriteFile(url, fuAlternative.PostedFile.InputStream);
        //        Refresh(Selection.SelectedItem);
        //    }
        //}
	}
}
