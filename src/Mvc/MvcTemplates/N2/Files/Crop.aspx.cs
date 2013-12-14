using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Edit.Web;
using N2.Web.Drawing;
using N2.Resources;
using N2.Edit.FileSystem;
using N2.Configuration;
using System.Drawing;
using System.IO;

namespace N2.Management.Files
{
    public partial class Crop : EditPage
    {
        protected string originalImagePath;
        protected string settings;

        private ImageSizesCollection sizes;
        private IFileSystem fs;
        private string baseImagePath;
        private string imageSize;
        protected ImageSizeElement size;
            
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            sizes = Engine.Resolve<EditSection>().Images.Sizes;
            fs = Engine.Resolve<IFileSystem>();

            ImagesUtility.SplitImageAndSize(Selection.SelectedItem.Url, sizes.GetSizeNames(), out baseImagePath, out imageSize);
            originalImagePath = fs.GetExistingImagePath(baseImagePath, "original");

            size = sizes.FirstOrDefault(s => s.Name == imageSize);

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.JQuery();
            Page.StyleSheet("{ManagementUrl}/Files/Css/Files.css");
            Page.StyleSheet("{ManagementUrl}/Files/Css/jquery.jCrop.min.css");
            Page.JavaScript("{ManagementUrl}/Files/Js/jquery.jCrop.min.js");

            if (size != null)
            {
                var s = new Dictionary<string, object>();
                if (size.Mode == ImageResizeMode.Fill)
                    s["aspectRatio"] = (int)(size.Width / size.Height);

                settings = N2.Web.WebExtensions.ToJson(s);
            }
        }

        protected void OnSaveCommand(object sender, CommandEventArgs args)
        {
            int x = int.Parse(Request["x"]);
            int y = int.Parse(Request["y"]);
            int w = int.Parse(Request["w"]);
            int h = int.Parse(Request["h"]);

            using (var input = GetInputStream(fs, originalImagePath))
            using (var output = fs.OpenFile(Selection.SelectedItem.Url))
            {
                var parameters = ImageResizeParameters.Fill(new Rectangle(x, y, w, h), size.Width, size.Height);
                Engine.Resolve<ImageResizer>().Resize(input, parameters, output);
            }

            Response.Redirect(GetPreviewUrl(Selection.SelectedItem));
        }

        private Stream GetInputStream(IFileSystem fs, string url)
        {
            byte[] image;
            using (var s = fs.OpenFile(url))
            {
                image = new byte[s.Length];
                s.Read(image, 0, image.Length);
            }

            return new MemoryStream(image);
        }
    }
}
