using System;
using System.IO;

namespace N2.Addons.Wiki.UI.Views
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
                    Response.Redirect(Server.UrlDecode(Request["returnUrl"]));
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
