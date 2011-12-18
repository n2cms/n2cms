using System;
using System.Collections.Generic;
using System.Linq;
using N2.Persistence;
using N2.Persistence.NH;

namespace N2.Tests.Fakes
{
	public class FakeContentItemRepository : FakeRepository<ContentItem>, IContentItemRepository
	{
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


	public class FakeRepository<TEntity> : INHRepository<TEntity> where TEntity : class
	{
		public string lastOperation;
		public int maxID;
		public Dictionary<object, TEntity> database = new Dictionary<object, TEntity>();

		#region IRepository<TKey,TEntity> Members

		public TEntity Get(object id)
		{
			lastOperation = "Get(" + id + ")";

			if (database.ContainsKey(id))
				return database[id];
			return null;
		}

		public T Get<T>(object id)
		{
			lastOperation = "Get<" + typeof(T).Name + ">(" + id + ")";

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
			lastOperation = "Load(" + id + ")";

			return database[id];
		}

		public void Delete(TEntity entity)
		{
			lastOperation = "Delete(" + entity + ")";

			database.Remove(GetKey(entity));
		}

		public virtual void Save(TEntity entity)
		{
			lastOperation = "Save(" + entity + ")";

			object key = GetKey(entity);
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
			lastOperation = "Update(" + entity + ")";

			database[GetKey(entity)] = entity;
		}

		public void SaveOrUpdate(TEntity entity)
		{
			lastOperation = "SaveOrUpdate(" + entity + ")";

			Save(entity);
		}

		public bool Exists()
		{
			lastOperation = "Exists()";

			return true;
		}

		public long Count()
		{
			lastOperation = "Count()";

			return database.Count;
		}

		public void Flush()
		{
			lastOperation = "Flush()";
		}

		private class FakeTransaction : ITransaction
		{

			#region ITransaction Members

			public void Commit()
			{
			}

			public void Rollback()
			{
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
			}

			#endregion
		}

		public ITransaction BeginTransaction()
		{
			lastOperation = "BeginTransaction()";

			return new FakeTransaction();
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			lastOperation = "Dispose()";
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
			throw new NotImplementedException();
		}
	}
}
