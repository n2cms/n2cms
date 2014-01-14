using System;
using System.Web.UI.WebControls;
using N2.Details;
using N2.Integrity;
using N2.Persistence;

namespace N2.Templates.Mvc.Models.Parts
{
    [PartDefinition("Random image",
        IconUrl = "~/Content/Img/photos.png")]
    [NotVersionable]
    public class RandomImage : PartBase
    {
        [DisplayableImage(CssClass = "main")]
        public virtual string RandomImageUrl
        {
            get
            {
                string[] images = Images.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (images.Length > 0)
                    return images[(new Random()).Next(images.Length)];
                else
                    return string.Empty;
            }
        }

        [EditableText("Images", 100, Rows = 8, Columns = 40, TextMode = TextBoxMode.MultiLine)]
        public virtual string Images
        {
            get { return (string) (GetDetail("Images") ?? string.Empty); }
            set { SetDetail("Images", value, string.Empty); }
        }
    }
}
