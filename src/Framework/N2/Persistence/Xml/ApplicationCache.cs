using N2.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Xml
{
	[Service]
	public class ApplicationCache<TEntity>
		where TEntity : class
	{
		protected Dictionary<object, CacheBox<IEnumerable<object>>> queryCache = new Dictionary<object, CacheBox<IEnumerable<object>>>();
		protected Dictionary<object, CacheBox<TEntity>> entityCache = new Dictionary<object, CacheBox<TEntity>>();
		private Func<TEntity, TEntity> dehydrate;
		
		public ApplicationCache(Func<TEntity, TEntity> dehydrate)
		{
			this.dehydrate = dehydrate;
		}

		public virtual TEntity Get(object id)
		{
			CacheBox<TEntity> box;
			if (entityCache.TryGetValue(id, out box))
				return box.Value;

			return null;
		}

		public virtual IEnumerable<object> GetQuery(object query)
		{
			CacheBox<IEnumerable<object>> box;
			if (queryCache.TryGetValue(query, out box))
				return box.Value;

			return null;
		}

		public virtual void Set(object id, TEntity entity)
		{
			entityCache[id] = new CacheBox<TEntity> { Value = dehydrate(entity) };
		}

		public virtual void SetQuery(object query, IEnumerable<object> ids)
		{
			queryCache[query] = new CacheBox<IEnumerable<object>> { Value = ids };
		}

		public virtual void Remove(object id)
		{
			if (id == null)
				return;
			entityCache.Remove(id);
			queryCache.Remove(id);
		}

		public virtual void Clear(bool entityCache = true, bool queryCache = true)
		{
			if (entityCache)
				this.entityCache = new Dictionary<object, CacheBox<TEntity>>();
			if (queryCache)
				this.queryCache = new Dictionary<object, CacheBox<IEnumerable<object>>>();
		}

		public virtual bool Exists(object id)
		{
			return entityCache.ContainsKey(id);
		}
	}
}
