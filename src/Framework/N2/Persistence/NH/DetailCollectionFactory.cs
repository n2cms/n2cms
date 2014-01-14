using N2.Collections;
using N2.Details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.NH
{
    public class DetailCollectionFactory : ContentListFactory<DetailCollection>
    {
        public override object Instantiate(int anticipatedSize)
        {
            return new DetailCollectionList();
        }
        public override NHibernate.Collection.IPersistentCollection Instantiate(NHibernate.Engine.ISessionImplementor session, NHibernate.Persister.Collection.ICollectionPersister persister)
        {
            return new PersistentDetailCollectionList(session);
        }
        public override NHibernate.Collection.IPersistentCollection Wrap(NHibernate.Engine.ISessionImplementor session, object collection)
        {
            return new PersistentDetailCollectionList(session, (IList<DetailCollection>)collection);
        }
    }
}
