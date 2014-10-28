using System;
using N2.Collections;
using N2.Templates.Items;
using System.Linq;

namespace N2.Templates.UI.Views
{
    public partial class ImageGallery : Web.UI.TemplatePage<Templates.Items.ImageGallery>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
			
            N2.Resources.Register.JQuery(this);
            rptImages.DataSource = CurrentPage.Children.WhereAccessible().OfType<GalleryItem>();
            rptImages.DataBind();
        }
    }
}
