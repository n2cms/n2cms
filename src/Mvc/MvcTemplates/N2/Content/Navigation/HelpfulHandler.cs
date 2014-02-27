using System.Collections.Specialized;
using System.Web;
using N2.Engine;
using N2.Web;

namespace N2.Edit.Navigation
{
    public abstract class HelpfulHandler : IHttpHandler
    {
        protected IEngine Engine
        {
            get { return N2.Context.Current; }
        }

        public virtual bool IsReusable
        {
            get { return true; }
        }

        public abstract void ProcessRequest(HttpContext context);

        protected ContentItem GetSelectedItem(NameValueCollection queryString)
        {
            string path = queryString[SelectionUtility.SelectedQueryKey];
            return N2.Context.Current.Resolve<N2.Edit.Navigator>().Navigate(path);
        }
    }
}
