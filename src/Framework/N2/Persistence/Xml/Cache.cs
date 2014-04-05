using N2.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Xml
{
	[Service]
	public class Cache<TEntity>
		where TEntity : class
	{
		protected IDictionary<object, Box<TEntity>> cache = new Dictionary<object, Box<TEntity>>();

		public virtual TEntity Get(object id, Func<TEntity> factory = null)
		{
			Box<TEntity> box;
			if (cache.TryGetValue(id, out box))
				return box.Entity;

			if (factory == null)
				return null;

			box = new Box<TEntity> { Entity = factory() };
			cache[id] = box;
			return box.Entity;
		}
		
		public virtual void Set(object id, TEntity entity)
		{
			cache[id] = new Box<TEntity> { Entity = entity };
		}

		public virtual void Remove(object id = null)
		{
			if (id != null)
				cache.Remove(id);
			else
				cache = new Dictionary<object, Box<TEntity>>();
		}

		public virtual IEnumerable<TEntity> Values
		{
			get { return cache.Values.Where(v => !v.IsEmpty).ToList().Select(v => v.Entity); }
		}

		public virtual IEnumerable<object> Keys
		{
			get { return cache.Keys.Where(k => !cache[k].IsEmpty); }
		}

		public virtual bool Exists(object id)
		{
			return cache.ContainsKey(id);
		}

		public virtual int Count
		{
			get { return cache.Count; }
		}
	}
}
