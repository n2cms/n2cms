using System;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls.Adapters;
using N2.Web;
using N2.Templates.Configuration;
    
namespace N2.Templates.Web.Adapters
{
    /// <summary>
    /// Adapts asp:image controls by changing their source to an image resizing handler that resizes the images on the server side.
    /// </summary>
    public class ImageAdapter : WebControlAdapter
    {
        static Url ImageHandlerUrl = "~/Templates/UI/Image.ashx";

        static ImageAdapter()
        {
            TemplatesSection config = ConfigurationManager.GetSection("n2/templates") as TemplatesSection;
            if(config != null && !string.IsNullOrEmpty(config.ImageHandlerPath))
            {
                ImageHandlerUrl = config.ImageHandlerPath;
            }
        }

        protected System.Web.UI.WebControls.Image ImageControl
        {
            get { return base.Control as System.Web.UI.WebControls.Image; }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!ImageControl.Height.IsEmpty || !ImageControl.Width.IsEmpty)
            {
                string thumbnailUrl = GetResizedImageUrl(ImageControl.ImageUrl, ImageControl.Width.Value, ImageControl.Height.Value);

                writer.WriteBeginTag("img");
                writer.WriteAttribute("alt", ImageControl.AlternateText);
                writer.WriteAttribute("src", thumbnailUrl);
                writer.WriteEndTag("img");
            }
            else if (ImageControl.ImageUrl.Length > 0)
                base.Render(writer);
        }
        
        /// <summary>Returns the path to an image handler that resizes the given image to the appropriate size.</summary>
        /// <param name="imageUrl">The image to resize.</param>
        /// <param name="width">The maximum width.</param>
        /// <param name="height">The maximum height.</param>
        /// <returns>The path to a handler that performs resizing of the image.</returns>
        public static string GetResizedImageUrl(string imageUrl, double width, double height)
        {
            string fileExtension = VirtualPathUtility.GetExtension(Url.PathPart(imageUrl));
            bool isAlreadyImageHandler = string.Equals(fileExtension, ".ashx", StringComparison.OrdinalIgnoreCase);
            
            if (isAlreadyImageHandler) return Url.ToAbsolute(imageUrl);

            Url url = ImageHandlerUrl.SetQueryParameter("img", Url.ToAbsolute(imageUrl));
            if(width > 0) url = url.SetQueryParameter("w", (int)width);
            if(height > 0) url = url.SetQueryParameter("h", (int)height);
            
            return url;
        }
    }
}
