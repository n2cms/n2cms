using System;
using System.Web;

namespace N2.Templates.UI.Parts
{
    public partial class SocialBookmarks : Web.UI.TemplateUserControl<Templates.Items.AbstractPage, N2.Templates.Items.SocialBookmarks>
    {
        protected override void OnInit(EventArgs e)
        {
            DataBind();
			
            base.OnInit(e);
        }

        protected string BookmarkUrl
        {
            get { return Server.UrlEncode("http://" + Request.Url.Authority + CurrentPage.Url); }
        }

        protected string BookmarkTitle
        {
            get { return Server.UrlEncode(CurrentPage.Title); }
        }

        protected string GetLinkContents(string icon, string text)
        {
            string format = CurrentItem.ShowText
                                ? "<img src='{0}' alt='' /> <span>{1}</span>"
                                : "<img src='{0}' alt='{1}' />";

            return string.Format(format, VirtualPathUtility.ToAbsolute("~/Templates/UI/Img/" + icon), text);
        }
    }
}