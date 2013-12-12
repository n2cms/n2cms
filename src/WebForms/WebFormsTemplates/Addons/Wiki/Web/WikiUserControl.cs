using N2.Security;
using N2.Web.UI;

namespace N2.Addons.Wiki.Web
{
    public class WikiUserControl<T> : ContentUserControl<T>, IWikiTemplate
        where T:Items.WikiBase, IArticle
    {
        protected bool IsAuthorized
        {
            get
            {
                if (Engine.SecurityManager.IsEditor(Page.User))
                    return true;

                return PermissionMap.IsInRoles(Page.User, CurrentPage.WikiRoot.ModifyRoles);
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

        ViewContext viewed = null;
        public ViewContext Viewed 
        {
            get { return viewed ?? (viewed = new ViewContext { Article = CurrentPage as IArticle, Fragment = null }); }
            set { viewed = value; }
        }
    }
}
