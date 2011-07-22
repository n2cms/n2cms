using System.Collections.Generic;
using System.Linq;
using N2.Collections;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Linq;

namespace N2.Persistence.NH
{
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

		public IQueryable<T> FindParts(string zoneName)
		{
			if (this.WasInitialized)
				return this.Where(i => i.ZoneName == zoneName).AsQueryable();

			if (zoneName == null)
				//return Query().Where(i => i.ZoneName == null); 
				return Session.CreateFilter(this, "where ZoneName is null").List<T>().AsQueryable();
			else
				//return Query().Where(i => i.ZoneName == zoneName); 
				return Session.CreateFilter(this, "where ZoneName = :zoneName").SetParameter("zoneName", zoneName).List<T>().AsQueryable();
		}

		public IQueryable<T> FindNavigatablePages()
		{
			if (this.WasInitialized)
				return FindPages().Where(p => new VisibleFilter().Match(p) && new PublishedFilter().Match(p)).AsQueryable();

			//var now = Utility.CurrentTime();
			//return Query().Where(i => i.ZoneName == null)
			//    .Where(i => i.Visible == true)
			//    .Where(i => i.Published <= now)
			//    .Where(i => i.Expires == null || now < i.Expires);
			return Session.CreateFilter(this, "where ZoneName is null and Visible = 1 and Published <= :published and (Expires is null or Expires > :expires)")
				.SetParameter("published", Utility.CurrentTime())
				.SetParameter("expires", Utility.CurrentTime())
				.List<T>().AsQueryable();
		}

		public IQueryable<T> FindPages()
		{
			if (this.WasInitialized)
				return this.Where(i => i.ZoneName == null).AsQueryable();

			//return Query().Where(i => i.ZoneName == null);
			return Session.CreateFilter(this, "where ZoneName is null").List<T>().AsQueryable();
		}

		public IQueryable<T> FindParts()
		{
			if (this.WasInitialized)
				return this.Where(i => i.ZoneName != null).AsQueryable();

			//return Query().Where(i => i.ZoneName != null);
			return Session.CreateFilter(this, "where ZoneName is not null").List<T>().AsQueryable();
		}

		public IEnumerable<string> FindZoneNames()
		{
			if (this.WasInitialized)
				return this.Select(i => i.ZoneName).Distinct();

			//return Query().Select(i => i.ZoneName).Distinct();
			return Session.CreateFilter(this, "select distinct ZoneName").List<string>().AsQueryable();
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

		#endregion

		private new ISession Session
		{
			get { return ((ISession)base.Session); }
		}
	}
}
