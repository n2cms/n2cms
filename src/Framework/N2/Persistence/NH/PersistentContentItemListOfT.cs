using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using NHibernate.Collection.Generic;
using NHibernate.UserTypes;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Collection;
using NHibernate;
using N2.Collections;

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

		public IList<T> FindByZone(string zoneName)
		{
			if (this.WasInitialized)
				return this.Where(i => i.ZoneName == zoneName).ToList();

			var session = ((ISession)Session);
			if (zoneName == null)
				return session.CreateFilter(this, "where ZoneName is null").List<T>();
			else
				return session.CreateFilter(this, "where ZoneName=:zoneName").SetParameter("zoneName", zoneName).List<T>();
		}

		#endregion
	}
}
