using System.Web.UI.HtmlControls;

namespace N2.Templates.UI.Parts
{
    public partial class Subscribe : Web.UI.TemplateUserControl<ContentItem, Templates.Items.Subscribe>
    {
        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);

            if(CurrentItem.SelectedFeed != null)
            {
                HtmlLink link = new HtmlLink();
                link.Attributes["rel"] = "alternate";
                link.Attributes["type"] = "application/rss+xml";
                link.Attributes["title"] = CurrentItem.SelectedFeed.Title;
                link.Href = CurrentItem.SelectedFeed.Url;

                Page.Header.Controls.Add(link);
            }
        }
    }
}
