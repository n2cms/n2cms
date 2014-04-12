using N2.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Xml
{
	[Service]
	public class SessionCache<TEntity>
		where TEntity : class
	{
		protected Dictionary<object, CacheBox<IEnumerable<object>>> queryCache = new Dictionary<object, CacheBox<IEnumerable<object>>>();
		protected Dictionary<object, CacheBox<TEntity>> entityCache = new Dictionary<object, CacheBox<TEntity>>();
		private ApplicationCache<TEntity> secondLevelCache;
		private Func<TEntity, TEntity> hydrate;

		public SessionCache(ApplicationCache<TEntity> secondLevelCache, Func<TEntity, TEntity> hydrate)
		{
			this.secondLevelCache = secondLevelCache;
			this.hydrate = hydrate;
		}

		public virtual TEntity Get(object id, Func<object, TEntity> factory = null)
		{
			CacheBox<TEntity> box;
			if (entityCache.TryGetValue(id, out box))
				return box.Value;

			var entity = secondLevelCache.Get(id);

			if (entity != null)
			{
				entityCache[id] = new CacheBox<TEntity> { Value = entity };
				hydrate(entity);
				return entity;
			}

			entity = factory(id);
			Set(id, entity);

			return entity;
		}

		public virtual IEnumerable<TEntity> Query(object query, Func<IEnumerable<Tuple<object, TEntity>>> resultsFactory = null, Func<object, TEntity> entityFactory = null)
		{
			CacheBox<IEnumerable<object>> box;
			if (queryCache.TryGetValue(query, out box))
				return box.Value.Select(id => Get(id, entityFactory));

			var cachedIds = secondLevelCache.GetQuery(query);

			if (cachedIds != null)
			{
				return cachedIds.Select(id => Get(id, entityFactory));
			}

			if (resultsFactory == null)
				return null;
			if (entityFactory == null)
				return null;

			var results = resultsFactory();
			var ids = new List<object>();
			var entities = new List<TEntity>();
			foreach (var result in results)
			{
				ids.Add(result.Item1);
				if (!Exists(result.Item1))
					Set(result.Item1, result.Item2);
				entities.Add(result.Item2);
			}
			box = new CacheBox<IEnumerable<object>> { Value = ids };
			queryCache[query] = box;
			secondLevelCache.SetQuery(query, ids);
			return entities;
		}

		public virtual void Set(object id, TEntity entity)
		{
			entityCache[id] = new CacheBox<TEntity> { Value = entity };
			secondLevelCache.Set(id, entity);
		}

		public virtual void Remove(object id)
		{
			if (id == null)
				return;
			entityCache.Remove(id);
			queryCache.Remove(id);

			secondLevelCache.Remove(id);
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
