using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Engine;
using NHibernate.Collection;
using NHibernate.Persister.Collection;

namespace N2.Persistence.NH
{
	public class ContentItemListFactory<T> : ContentListFactory<T> where T : ContentItem
	{
		public override IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister)
		{
			return new PersistentContentItemList<T>(session);
		}

		public override IPersistentCollection Wrap(ISessionImplementor session, object collection)
		{
			return new PersistentContentItemList<T>(session, (IList<T>)collection);
		}
	}
}
