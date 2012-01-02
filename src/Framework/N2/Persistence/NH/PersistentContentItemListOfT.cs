using System.Collections.Generic;
using System.Linq;
using N2.Collections;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Linq;
using System.Diagnostics;

namespace N2.Persistence.NH
{
	[DebuggerDisplay("PersistentContentItemList: Count = {Count}")]
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

		public int EnclosingItemID
		{
			get { return (Owner as ContentItem).ID; }
		}

		#region IZonedList<T> Members

		public IEnumerable<T> FindParts(string zoneName)
		{
			if (this.WasInitialized)
				return this.Where(i => i.ZoneName == zoneName).OrderBy(i => i.SortOrder);

			if (zoneName == null)
				//return Query().Where(i => i.ZoneName == null); 
				return Session.CreateFilter(this, "where ZoneName is null order by SortOrder").SetCacheable(true).List<T>();
			else
				//return Query().Where(i => i.ZoneName == zoneName); 
                return Session.CreateFilter(this, "where ZoneName = :zoneName order by SortOrder").SetCacheable(true).SetParameter("zoneName", zoneName).List<T>();
		}

		public IEnumerable<T> FindNavigatablePages()
		{
			if (this.WasInitialized)
				return FindPages().Where(p => new VisibleFilter().Match(p) && new PublishedFilter().Match(p)).OrderBy(i => i.SortOrder);

			//var now = Utility.CurrentTime();
			//return Query().Where(i => i.ZoneName == null)
			//    .Where(i => i.Visible == true)
			//    .Where(i => i.Published <= now)
			//    .Where(i => i.Expires == null || now < i.Expires);
			return Session.CreateFilter(this, "where ZoneName is null and Visible = 1 and Published <= :published and (Expires is null or Expires > :expires) order by SortOrder")
				.SetParameter("published", Utility.CurrentTime())
				.SetParameter("expires", Utility.CurrentTime())
                .SetCacheable(true)
                .List<T>();
		}

		public IEnumerable<T> FindPages()
		{
			if (this.WasInitialized)
				return this.Where(i => i.ZoneName == null).OrderBy(i => i.SortOrder);

			//return Query().Where(i => i.ZoneName == null);
            return Session.CreateFilter(this, "where ZoneName is null order by SortOrder").SetCacheable(true).List<T>();
		}

		public IEnumerable<T> FindParts()
		{
			if (this.WasInitialized)
				return this.Where(i => i.ZoneName != null).OrderBy(i => i.SortOrder);

			//return Query().Where(i => i.ZoneName != null);
            return Session.CreateFilter(this, "where ZoneName is not null order by SortOrder").SetCacheable(true).List<T>();
		}

		public IEnumerable<string> FindZoneNames()
		{
			if (this.WasInitialized)
				return this.Select(i => i.ZoneName).Distinct();

			//return Query().Select(i => i.ZoneName).Distinct();
            return Session.CreateFilter(this, "select distinct ZoneName").SetCacheable(true).List<string>();
		}

		#endregion

		#region IQueryableList<T> Members

		public override IQueryable<T> Query()
		{
			if (WasInitialized)
				return this.AsQueryable<T>();

			return Session.Query<T>().Where(i => i.Parent.ID == EnclosingItemID);
		}

		#endregion

		private new ISession Session
		{
			get { return ((ISession)base.Session); }
		}
	}
}
