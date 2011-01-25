using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using NHibernate.Collection.Generic;
using NHibernate.Engine;
using NHibernate.UserTypes;
using NHibernate.Collection;
using NHibernate.Persister.Collection;
using NHibernate;
using N2.Collections;

namespace N2.Collections
{
	public interface IPageableList<T> : IList<T>, IList, ICollection where T : class
	{
		// Summary:
		//     Gets or sets the element at the specified index.
		//
		// Parameters:
		//   index:
		//     The zero-based index of the element to get or set.
		//
		// Returns:
		//     The element at the specified index.
		//
		// Exceptions:
		//   System.ArgumentOutOfRangeException:
		//     index is not a valid index in the System.Collections.Generic.IList<T>.
		//
		//   System.NotSupportedException:
		//     The property is set and the System.Collections.Generic.IList<T> is read-only.
		new T this[int index] { get; set; }

		//
		// Summary:
		//     Removes all items from the System.Collections.Generic.ICollection<T>.
		//
		// Exceptions:
		//   System.NotSupportedException:
		//     The System.Collections.Generic.ICollection<T> is read-only.
		new void Clear();

		// Summary:
		//     Gets the number of elements contained in the System.Collections.Generic.ICollection<T>.
		//
		// Returns:
		//     The number of elements contained in the System.Collections.Generic.ICollection<T>.
		new int Count { get; }

		IEnumerable<T> GetRange(int skip, int take);
	}

	public class PageableList<T> : IPageableList<T> where T : class
	{
		private List<T> inner = new List<T>();

		public IEnumerable<T> GetRange(int skip, int take)
		{
			return inner.Skip(skip).Take(take);
		}

		public int IndexOf(T item)
		{
			return inner.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			inner.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			inner.RemoveAt(index);
		}

		public T this[int index]
		{
			get { return inner[index]; }
			set { inner[index] = value; }
		}

		public void Add(T item)
		{
			inner.Add(item);
		}

		public void Clear()
		{
			inner.Clear();
		}

		public bool Contains(T item)
		{
			return inner.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			inner.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return inner.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(T item)
		{
			return inner.Remove(item);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return inner.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return inner.GetEnumerator();
		}

		public void CopyTo(Array array, int index)
		{
			T[] arr = new T[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				arr[i] = (T)array.GetValue(i);
			}

			inner.CopyTo(arr, index);
		}

		int ICollection.Count
		{
			get { return inner.Count; }
		}

		// The IsSynchronized Boolean property returns True if the 
		// collection is designed to be thread safe; otherwise, it returns False.
		public bool IsSynchronized
		{
			get { return false; }
		}

		public object SyncRoot
		{
			get { return this; }
		}

		#region IList Members

		int IList.Add(object value)
		{
			int index = IndexOf(value as T);
			Add(value as T);
			return index;
		}

		void IList.Clear()
		{
			Clear();
		}

		bool IList.Contains(object value)
		{
			return Contains(value as T);
		}

		int IList.IndexOf(object value)
		{
			return IndexOf(value as T);
		}

		void IList.Insert(int index, object value)
		{
			Insert(index, value as T);
		}

		bool IList.IsFixedSize
		{
			get { return false; }
		}

		bool IList.IsReadOnly
		{
			get { return false; }
		}

		void IList.Remove(object value)
		{
			Remove(value as T);
		}

		void IList.RemoveAt(int index)
		{
			RemoveAt(index);
		}

		object IList.this[int index]
		{
			get { return this[index]; }
			set { this[index] = value as T; }
		}

		#endregion
	}
}

namespace N2.Persistence.NH
{
	// Kudos to john (http://stackoverflow.com/questions/876976/implementing-ipagedlistt-on-my-models-using-nhibernate)
	public class PageableListFactory<T> : IUserCollectionType where T : class
	{
		public PageableListFactory()
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

		public object Instantiate(int anticipatedSize)
		{
			return new PageableList<T>();
		}

		public IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister)
		{
			return new PersistentPageableList<T>(session);
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

		public IPersistentCollection Wrap(ISessionImplementor session, object collection)
		{
			return new PersistentPageableList<T>(session, (IList<T>)collection);
		}

		#endregion
	}

	public class PersistentPageableList<T> : PersistentGenericBag<T>, IPageableList<T> where T : class
	{

		public PersistentPageableList(ISessionImplementor session)
			: base(session)
		{
			this.sessionImplementor = session;
		}

		public PersistentPageableList(ISessionImplementor session, IList<T> collection)
			: base(session, collection)
		{
			this.sessionImplementor = session;
		}

		private ICollectionPersister collectionPersister = null;
		public PersistentPageableList<T> CollectionPersister(ICollectionPersister collectionPersister)
		{
			this.collectionPersister = collectionPersister;
			return this;
		}

		protected ISessionImplementor sessionImplementor = null;

		public virtual IEnumerable<T> GetRange(int skip, int take)
		{
			if (!this.WasInitialized)
			{
				IQuery pagedList = ((ISession)sessionImplementor)
					.CreateFilter(this, "")
					.SetFirstResult(skip)
					.SetMaxResults(take)
					.SetCacheable(true);

				return pagedList.Enumerable<T>();
			}

			return this.Skip(skip)
				.Take(take);
		}

		public new int Count
		{
			get
			{
				if (!this.WasInitialized)
				{
					return Convert.ToInt32(((ISession)sessionImplementor).CreateFilter(this, "select count(*)").UniqueResult());
				}

				return base.Count;
			}
		}

		new public T this[int index]
		{
			get { return base[index] as T; }
			set { base[index] = value; }
		}
	}
}
