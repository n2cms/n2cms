using System;
using System.Web;

namespace N2.Templates.UI.Parts
{
    public partial class SocialBookmarks : Web.UI.TemplateUserControl<ContentItem, N2.Templates.Items.SocialBookmarks>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            N2.Resources.Register.JQuery(Page);
        }

        protected string GetUrl()
        {
            string url = CurrentItem.LikeSite
                ? Request.Url.Scheme + "://" + Request.Url.Authority + Content.Traverse.StartPage.Url
                : Request.Url.ToString();

            return HttpUtility.HtmlAttributeEncode(url);
        }
    }
}
