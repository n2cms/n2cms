using System;
using N2.Templates.Items;
using N2.Templates.Services;
using N2.Web.UI;

namespace N2.Templates.UI.Views
{
    public partial class Feed : ContentPage<RssFeed>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Request["hungry"] == "yes")
            {
                Response.ContentType = "text/xml";
                Engine.Resolve<RssWriter>().Write(Response.Output, CurrentItem);
                Response.End();
            }
        }
    }
}