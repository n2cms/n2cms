using System;
using N2.Web.UI;

namespace N2.Addons.Wiki.UI.Views
{
    public class WikiTemplatePage : ContentPage<Items.WikiArticle>
    {
        protected bool IsAuthorized
        {
            get
            {
                foreach (string role in CurrentPage.WikiRoot.ModifyRoles)
                    if (User.IsInRole(role))
                        return true;
                return false;
            }
        }

        protected string CurrentAction
        {
            get { return Engine.RequestContext.CurrentPath.Action; }
        }

        protected string CurrentArguments
        {
            get { return Engine.RequestContext.CurrentPath.Argument; }
        }

        protected override void OnPreRender(EventArgs e)
        {
            N2.Resources.Register.StyleSheet(this, "~/Addons/Wiki/UI/Css/Wiki.css");
            base.OnPreRender(e);
        }
    }
}
