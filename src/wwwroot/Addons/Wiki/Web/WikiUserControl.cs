using N2.Templates.Items;
using N2.Templates.Web.UI;
using N2.Web;

namespace N2.Addons.Wiki.Web
{
    public class WikiUserControl<T> : TemplateUserControl<T>, IWikiTemplate
        where T:AbstractContentPage, IArticle
    {
        protected bool IsAuthorized
        {
            get
            {
                if (Engine.SecurityManager.IsEditor(Page.User))
                    return true;

                return Engine.SecurityManager.IsAuthorized(Page.User, CurrentPage.WikiRoot.ModifyRoles);
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
