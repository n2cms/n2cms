using System.Collections;
using System.Collections.Generic;
using N2.Collections;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.UserTypes;

namespace N2.Persistence.NH
{
    public class ContentListFactory<T> : IUserCollectionType where T : class, INameable
    {
        public ContentListFactory()
        {
        }

        #region IUserCollectionType Members

        public bool Contains(object collection, object entity)
        {
            return ((IList<T>)collection).Contains((T)entity);
        }

        public IEnumerable GetElements(object collection)
        {
            return (IEnumerable)collection;
        }

        public object IndexOf(object collection, object entity)
        {
            return ((IList<T>)collection).IndexOf((T)entity);
        }

        public virtual object Instantiate(int anticipatedSize)
        {
            return new ContentList<T>();
        }

        public virtual IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister)
        {
            return new PersistentContentList<T>(session);
        }

        public object ReplaceElements(object original, object target, ICollectionPersister persister,
                object owner, IDictionary copyCache, ISessionImplementor session)
        {
            IList<T> result = (IList<T>)target;

            result.Clear();
            foreach (object item in ((IEnumerable)original))
            {
                result.Add((T)item);
            }

            return result;
        }

        public virtual IPersistentCollection Wrap(ISessionImplementor session, object collection)
        {
            return new PersistentContentList<T>(session, (IList<T>)collection);
        }

        #endregion
    }
}
