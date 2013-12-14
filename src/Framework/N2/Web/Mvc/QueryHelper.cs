using System.Collections.Generic;
using System.Web.Mvc;
using N2.Persistence.Finder;
using N2.Persistence.NH;
using N2.Web.Mvc.Html;
using N2.Linq;
using System.Linq;

namespace N2.Web.Mvc
{
    public class QueryHelper<T> : IQueryable<T> where T : ContentItem
    {
        HtmlHelper html;
        IQueryable<T> innerQuery;

        public QueryHelper(HtmlHelper html)
        {
            this.html = html;
        }

        //public IQueryAction Descendants(ContentItem root = null)
        //{
        //    if (root == null)
        //        root = html.CurrentItem();
        //    return html.ResolveService<IItemFinder>().Where.AncestralTrail.Like(Utility.GetTrail(root) + "%");
        //}

        IQueryable<T> InnerQuery
        {
            get { return innerQuery ?? (innerQuery = html.ContentEngine().QueryItems<T>()); }
        }

        //public IEnumerable<ContentItem> Text(string text)
        //{
        //    return html.ResolveService<ISessionProvider>().OpenSession.FullText(text).Enumerable<ContentItem>();
        //}

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return InnerQuery.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return InnerQuery.GetEnumerator();
        }

        #endregion

        #region IQueryable Members

        System.Type IQueryable.ElementType
        {
            get { return InnerQuery.ElementType; }
        }

        System.Linq.Expressions.Expression IQueryable.Expression
        {
            get { return InnerQuery.Expression; }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return InnerQuery.Provider; }
        }

        #endregion
    }
}
