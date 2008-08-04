using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;

namespace N2.Templates.Wiki.UI
{
    public partial class Upload : WikiTemplatePage
    {
        protected string FileName
        {
            get { return Request["parameter"]; }
        }

        protected override void OnInit(EventArgs e)
        {
            cvUpload.IsValid = HasValidExtension(FileName) && !LookSuspicious(FileName);
            phUpload.Visible = cvUpload.IsValid;

            base.OnInit(e);
        }

        protected void btnUpload_Click(object sender, EventArgs args)
        {
            cvUpload.IsValid = fuUpload.PostedFile.ContentLength > 0;
            cvExtension.IsValid = HasValidExtension(fuUpload.PostedFile.FileName)
                && Path.GetExtension(fuUpload.PostedFile.FileName).Equals(Path.GetExtension(FileName), StringComparison.InvariantCultureIgnoreCase);
            if (cvUpload.IsValid && cvExtension.IsValid)
            {
                string uploadPath = MapPath(CurrentPage.WikiRoot.UploadFolder);
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);
                string filePath = Path.Combine(uploadPath, FileName);
                fuUpload.PostedFile.SaveAs(filePath);

                if (string.IsNullOrEmpty(Request["returnUrl"]))
                    Response.Redirect(CurrentPage.WikiRoot.Url);
                else
                    Response.Redirect(Request["returnUrl"]);
            }
        }

        private bool LookSuspicious(string name)
        {
            return name.Contains("..") || name.Contains("/") || name.Contains("\\");
        }

        private bool HasValidExtension(string name)
        {
            string extension = Path.GetExtension(name).ToLower();
            switch (extension)
            {
                case ".gif":
                case ".png":
                case ".jpg":
                case ".jpeg":
                    return true;
                default:
                    return false;
            }
        }
    }
}
