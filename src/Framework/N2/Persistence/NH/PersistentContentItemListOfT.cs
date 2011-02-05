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

		public IList<T> FindParts(string zoneName)
		{
			if (this.WasInitialized)
				return this.Where(i => i.ZoneName == zoneName).ToList();

			var session = ((ISession)Session);
			if (zoneName == null)
				return session.CreateFilter(this, "where ZoneName is null").List<T>();
			else
				return session.CreateFilter(this, "where ZoneName=:zoneName").SetParameter("zoneName", zoneName).List<T>();
		}

		public IList<T> FindPages()
		{
			if (this.WasInitialized)
				return this.Where(i => i.ZoneName == null).ToList();

			var session = ((ISession)Session);
			return session.CreateFilter(this, "where ZoneName is null").List<T>();
		}

		public IList<T> FindParts()
		{
			if (this.WasInitialized)
				return this.Where(i => i.ZoneName != null).ToList();

			var session = ((ISession)Session);
			return session.CreateFilter(this, "where ZoneName is not null").List<T>();
		}

		public IList<string> FindZoneNames()
		{
			if (this.WasInitialized)
				return this.Select(i => i.ZoneName).Distinct().ToList();

			var session = ((ISession)Session);
			return session.CreateFilter(this, "select distinct ZoneName").List<string>();
		}

		#endregion

		#region IQueryableList<T> Members

		public IQueryable<T> Query()
		{
			if (WasInitialized)
				return this.AsQueryable<T>();

			return ((ISession)Session).Query<T>().Where(i => i.Parent == Owner);
		}

		#endregion
	}
}
