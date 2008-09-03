using System;

namespace N2.Templates.UI.Views
{
    public partial class ImageGallery : Web.UI.TemplatePage<Templates.Items.ImageGallery>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            N2.Resources.Register.JQuery(this);
        }
    }
}