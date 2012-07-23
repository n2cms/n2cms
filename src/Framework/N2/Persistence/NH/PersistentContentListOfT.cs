using System;
using System.Collections.Generic;
using System.Linq;
using N2.Collections;
using NHibernate;
using NHibernate.Collection.Generic;
using NHibernate.Engine;

namespace N2.Persistence.NH
{
	public class PersistentContentList<T> : PersistentGenericBag<T>, IContentList<T> where T : class, INameable
	{
		public PersistentContentList(ISessionImplementor session)
			: base(session)
		{
		}

		public PersistentContentList(ISessionImplementor session, IList<T> collection)
			: base(session, collection)
		{
		}

		#region IList overrides

		new public int Count
		{
			get
			{
				if (this.WasInitialized)
					return base.Count;

                return Convert.ToInt32(((ISession)Session).CreateFilter(this, "select count(*)")
                    .SetCacheable(true).UniqueResult());
			}
		}

		new public T this[int index]
		{
			get { return base[index] as T; }
			set { base[index] = value; }
		}

		#endregion

		#region INamedList<T> Members

		public void Add(string key, T value)
		{
			EnsureName(key, value);

			Add(value);
		}

		private IList<T> List
		{
			get { return this; }
		}

		public bool ContainsKey(string key)
		{
			return List.Any(i => i.Name == key);
		}

		public ICollection<string> Keys
		{
			get { return List.Select(i => i.Name).ToList(); }
		}

		public bool Remove(string key)
		{
			var item = this[key];
			if (item != null)
				this.Remove(item);
			return item != null;
		}

		public bool TryGetValue(string key, out T value)
		{
			value = List.FirstOrDefault(i => i.Name == key);
			return value != null;
		}

		public ICollection<T> Values
		{
			get { return List.ToList(); }
		}

		public T this[string name]
		{
			get
			{
				return FindNamed(name);
			}
			set
			{
				EnsureName(name, value);

				var result = List.Select((item, index) => new { item, index }).FirstOrDefault(i => i.item.Name == name);
				if (result == null)
					Add(name, value);
				else
					this[result.index] = value;
			}
		}

		public T FindNamed(string name)
		{
			if (WasInitialized) return List.FirstOrDefault(i => string.Equals(i.Name, name, StringComparison.InvariantCultureIgnoreCase));

            return ((ISession)Session).CreateFilter(this, "where Name like :name")
				.SetParameter("name", name)
                .SetCacheable(true)
				.SetMaxResults(1)
				.List<T>()
				.FirstOrDefault();
		}

		#endregion

		#region IPageableList<T> Members

		public virtual IQueryable<T> FindRange(int skip, int take)
		{
			if (this.WasInitialized)
				return this.Skip(skip)
					.Take(take)
					.AsQueryable();

			//return Query().Skip(skip).Take(take);
			return ((ISession)Session)
				.CreateFilter(this, "")
				.SetFirstResult(skip)
				.SetMaxResults(take)
				.SetCacheable(true)
				.List<T>().AsQueryable();
		}

		#endregion

		#region IQueryableList<T> Members

		public virtual IQueryable<T> Query()
		{
			if (WasInitialized)
				return this.AsQueryable<T>();

			throw new NotSupportedException("Cannot query since we don't know the parent relation in this case.");
		}

		#endregion

		private static void EnsureName(string key, T value)
		{
			if (value.Name != key)
				throw new InvalidOperationException("Cannot add value with differnet name (" + key + " != " + value.Name + ")");
		}

		public IContentList<T> Clone()
		{
			return new ContentList<T>(this.ToList());
		}
	}
}
