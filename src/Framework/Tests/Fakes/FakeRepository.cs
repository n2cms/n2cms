using System;
using System.Collections.Generic;
using System.Linq;
using N2.Persistence;
using N2.Persistence.NH;
using N2.Persistence.Proxying;

namespace N2.Tests.Fakes
{
    public class FakeContentItemRepository : FakeRepository<ContentItem>, IContentItemRepository
    {
		public FakeContentItemRepository(IProxyFactory proxyFactory = null)
			: base(proxyFactory)
		{
		}

        public IEnumerable<DiscriminatorCount> FindDescendantDiscriminators(ContentItem ancestor)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ContentItem> FindDescendants(ContentItem ancestor, string discriminator)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ContentItem> FindReferencing(ContentItem linkTarget)
        {
            throw new NotImplementedException();
        }

        public int RemoveReferencesToRecursive(ContentItem target)
        {
            return 0;
        }
    }


    public class FakeRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private string lastOperation;
        public int maxID;
        public Dictionary<object, TEntity> database = new Dictionary<object, TEntity>();
        private FakeTransaction transaction;
		private IProxyFactory proxies;

		public FakeRepository(IProxyFactory proxyFactory = null)
		{
			this.proxies = proxyFactory ?? new InterceptingProxyFactory();
		}

        public string LastOperation
        {
            get { return lastOperation; }
            set { lastOperation = value; }
        }

        #region IRepository<TKey,TEntity> Members

        public TEntity Get(object id)
        {
            LastOperation = "Get(" + id + ")";

            if (database.ContainsKey(id))
                return database[id];
            return null;
        }

        public T Get<T>(object id)
        {
            LastOperation = "Get<" + typeof(T).Name + ">(" + id + ")";

            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> Find(string propertyName, object value)
        {
            return Find(new Parameter(propertyName, value));
        }

        public IEnumerable<TEntity> Find(params Parameter[] propertyValuesToMatchAll)
        {
            return database.Values.Where(item => propertyValuesToMatchAll.All(p => p.IsMatch(item)));
        }

        public TEntity Load(object id)
        {
            LastOperation = "Load(" + id + ")";

            return database[id];
        }

        public void Delete(TEntity entity)
        {
            LastOperation = "Delete(" + entity + ")";

            database.Remove(GetKey(entity));
        }

        public virtual void Save(TEntity entity)
        {
            LastOperation = "Save(" + entity + ")";

            object key = GetKey(entity);
			proxies.OnSaving(entity);
            database[key] = entity;

            if (key is int)
                maxID = Math.Max(maxID, (int)key);
        }

        protected virtual object GetKey(TEntity entity)
        {
            var q = database.Keys.Where(k => database[k] == entity);
            if (q.Count() > 0)
                return q.Single();
            var p = entity.GetType().GetProperty("ID");
            object key = p.GetValue(entity, new object[0]);
            if (key is int && (int)key == 0)
                key = ++maxID;

            p.SetValue(entity, key, new object[0]);
            return key;
        }

        public void Update(TEntity entity)
        {
            LastOperation = "Update(" + entity + ")";

            database[GetKey(entity)] = entity;
        }

        public void SaveOrUpdate(TEntity entity)
        {
            LastOperation = "SaveOrUpdate(" + entity + ")";

            Save(entity);
        }

        public bool Exists()
        {
            LastOperation = "Exists()";

            return true;
        }

        public long Count()
        {
            LastOperation = "Count()";

            return database.Count;
        }

        public void Flush()
        {
            LastOperation = "Flush()";
        }

        private class FakeTransaction : ITransaction
        {

            #region ITransaction Members

            public void Commit()
            {
                Committed(this, new EventArgs());
            }

            public void Rollback()
            {
                Rollbacked(this, new EventArgs());
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                Disposed(this, new EventArgs());
            }

            #endregion


            /// <summary>Invoked after the transaction has been committed.</summary>
            public event EventHandler Committed = delegate { };

            /// <summary>Invoked after the transaction has been rollbacked.</summary>
            public event EventHandler Rollbacked = delegate { };

            /// <summary>Invoked after the transaction has closed and is disposed.</summary>
            public event EventHandler Disposed = delegate { };
        }

        public ITransaction BeginTransaction()
        {
            LastOperation = "BeginTransaction()";

            return transaction = new FakeTransaction();
        }

        public ITransaction GetTransaction()
        {
            LastOperation = "GetTransaction()";

            return transaction;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            LastOperation = "Dispose()";
        }

        #endregion

        #region INHRepository<int,TEntity> Members

        public ICollection<TEntity> FindAll(NHibernate.Criterion.Order order, params NHibernate.Criterion.ICriterion[] criteria)
        {
            throw new NotImplementedException();
        }

        public ICollection<TEntity> FindAll(NHibernate.Criterion.DetachedCriteria criteria, params NHibernate.Criterion.Order[] orders)
        {
            throw new NotImplementedException();
        }

        public ICollection<TEntity> FindAll(NHibernate.Criterion.DetachedCriteria criteria, int firstResult, int maxResults, params NHibernate.Criterion.Order[] orders)
        {
            throw new NotImplementedException();
        }

        public ICollection<TEntity> FindAll(NHibernate.Criterion.Order[] orders, params NHibernate.Criterion.ICriterion[] criteria)
        {
            throw new NotImplementedException();
        }

        public ICollection<TEntity> FindAll(params NHibernate.Criterion.ICriterion[] criteria)
        {
            throw new NotImplementedException();
        }

        public ICollection<TEntity> FindAll(int firstResult, int numberOfResults, params NHibernate.Criterion.ICriterion[] criteria)
        {
            throw new NotImplementedException();
        }

        public ICollection<TEntity> FindAll(int firstResult, int numberOfResults, NHibernate.Criterion.Order selectionOrder, params NHibernate.Criterion.ICriterion[] criteria)
        {
            throw new NotImplementedException();
        }

        public ICollection<TEntity> FindAll(int firstResult, int numberOfResults, NHibernate.Criterion.Order[] selectionOrder, params NHibernate.Criterion.ICriterion[] criteria)
        {
            throw new NotImplementedException();
        }

        public ICollection<TEntity> FindAll(string namedQuery, params Parameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public ICollection<TEntity> FindAll(int firstResult, int numberOfResults, string namedQuery, params Parameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public TEntity FindOne(params NHibernate.Criterion.ICriterion[] criteria)
        {
            throw new NotImplementedException();
        }

        public TEntity FindOne(NHibernate.Criterion.DetachedCriteria criteria)
        {
            throw new NotImplementedException();
        }

        public TEntity FindOne(string namedQuery, params Parameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public TEntity FindFirst(NHibernate.Criterion.DetachedCriteria criteria, params NHibernate.Criterion.Order[] orders)
        {
            throw new NotImplementedException();
        }

        public TEntity FindFirst(params NHibernate.Criterion.Order[] orders)
        {
            throw new NotImplementedException();
        }

        public bool Exists(NHibernate.Criterion.DetachedCriteria criteria)
        {
            throw new NotImplementedException();
        }

        public long Count(NHibernate.Criterion.DetachedCriteria criteria)
        {
            throw new NotImplementedException();
        }

        #endregion

        public IEnumerable<TEntity> Find(IParameter parameters)
        {
            return database.Values.Where(item => parameters.IsMatch(item));
        }
        
        public IEnumerable<IDictionary<string, object>> Select(IParameter parameters, params string[] properties)
        {
            return Find(parameters)
                .Select(r =>
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < properties.Length; i++)
                        row[properties[i]] = N2.Utility.GetProperty(r, properties[i]);
                    return row;
                });
        }

        public long Count(IParameter parameters)
        {
            return Find(parameters).Count();
        }
    }
}
