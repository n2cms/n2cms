using System.Collections.Generic;
using System.Linq;
using N2.Collections;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Linq;
using System.Diagnostics;
using NHibernate.Criterion;

namespace N2.Persistence.NH
{
    [DebuggerDisplay("PersistentContentItemList, Count = {Count}")]
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    public class PersistentContentItemList<T> : PersistentContentList<T>, IContentItemList<T> where T : ContentItem
    {
        public PersistentContentItemList(ISessionImplementor session)
            : base(session)
        {
        }

        public PersistentContentItemList(ISessionImplementor session, IList<T> collection)
            : base(session, collection)
        {
        }

        #region IZonedList<T> Members

        public IEnumerable<T> FindParts(string zoneName)
        {
            if (this.WasInitialized)
                return this.Where(i => i.ZoneName == zoneName).OrderBy(i => i.SortOrder);

            if (zoneName == null)
                return Session.CreateFilter(this, "where ZoneName is null order by SortOrder").SetCacheable(true).List<T>();
            else
                return Session.CreateFilter(this, "where ZoneName = :zoneName order by SortOrder").SetCacheable(true).SetParameter("zoneName", zoneName).List<T>();
        }

        public IEnumerable<T> FindNavigatablePages()
        {
            if (this.WasInitialized)
                return FindPages().Where(p => new VisibleFilter().Match(p) && new PublishedFilter().Match(p)).OrderBy(i => i.SortOrder);

            return Session.CreateFilter(this, "where ZoneName is null and Visible = 1 and State = :state order by SortOrder")
                .SetParameter("state", ContentState.Published)
                .SetCacheable(true)
                .List<T>();
        }

        public IEnumerable<T> FindPages()
        {
            if (this.WasInitialized)
                return this.Where(i => i.ZoneName == null).OrderBy(i => i.SortOrder);

            return Session.CreateFilter(this, "where ZoneName is null order by SortOrder").SetCacheable(true).List<T>();
        }

        public IEnumerable<T> FindParts()
        {
            if (this.WasInitialized)
                return this.Where(i => i.ZoneName != null).OrderBy(i => i.SortOrder);

            return Session.CreateFilter(this, "where ZoneName is not null order by SortOrder").SetCacheable(true).List<T>();
        }

        public IEnumerable<string> FindZoneNames()
        {
            if (this.WasInitialized)
                return this.Select(i => i.ZoneName).Distinct();

            return Session.CreateFilter(this, "select distinct ZoneName").SetCacheable(true).List<string>();
        }

        #endregion

        #region IQueryableList<T> Members

        public override IQueryable<T> Query()
        {
            if (WasInitialized)
                return this.AsQueryable<T>();

            var parent = Owner as ContentItem;
            return Session.Query<T>().Where(i => i.Parent == parent);
        }

        public virtual IEnumerable<T> Find(IParameter parameters)
        {
            return Session.CreateCriteria<T>(parameters)
                .Add(Expression.Eq("Parent", Owner))
                .List<T>();
        }

        public virtual int FindCount(IParameter parameters)
        {
            return (int)Session.CreateCriteria<T>(parameters)
                .Add(Expression.Eq("Parent", Owner))
                .SetProjection(Projections.RowCountInt64())
                .UniqueResult<long>();
        }

        public IEnumerable<IDictionary<string, object>> Select(IParameter parameters, params string[] properties)
        {
            var crit = Session.CreateCriteria<T>(parameters)
                .Add(Expression.Eq("Parent", Owner));
            return crit.SelectProperties(properties);
        }

        #endregion

        private new ISession Session
        {
            get { return ((ISession)base.Session); }
        }
    }
}
