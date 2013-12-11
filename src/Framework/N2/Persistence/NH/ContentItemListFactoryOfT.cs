using System.Collections.Generic;
using NHibernate.Collection;
using NHibernate.Engine;
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
