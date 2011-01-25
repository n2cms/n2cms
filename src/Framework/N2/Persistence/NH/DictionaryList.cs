using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using NHibernate.Collection.Generic;
using NHibernate.UserTypes;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Collection;
using NHibernate;
using N2.Collections;

namespace N2.Collections
{
	public interface INameable
	{
		string Name { get; }
	}

	public interface IDictionaryList<T> : IList<T>, IList, ICollection where T : class, INameable
	{
		// Summary:
		//     Gets the number of elements contained in the System.Collections.Generic.ICollection<T>.
		//
		// Returns:
		//     The number of elements contained in the System.Collections.Generic.ICollection<T>.
		new int Count { get; }

		//
		// Summary:
		//     Removes all items from the System.Collections.Generic.ICollection<T>.
		//
		// Exceptions:
		//   System.NotSupportedException:
		//     The System.Collections.Generic.ICollection<T> is read-only.
		new void Clear();

		// Summary:
		//     Gets an System.Collections.Generic.ICollection<T> containing the keys of
		//     the System.Collections.Generic.IDictionary<TKey,TValue>.
		//
		// Returns:
		//     An System.Collections.Generic.ICollection<T> containing the keys of the object
		//     that implements System.Collections.Generic.IDictionary<TKey,TValue>.
		ICollection<string> Keys { get; }
		//
		// Summary:
		//     Gets an System.Collections.Generic.ICollection<T> containing the values in
		//     the System.Collections.Generic.IDictionary<TKey,TValue>.
		//
		// Returns:
		//     An System.Collections.Generic.ICollection<T> containing the values in the
		//     object that implements System.Collections.Generic.IDictionary<TKey,TValue>.
		ICollection<T> Values { get; }

		// Summary:
		//     Gets or sets the element with the specified key.
		//
		// Parameters:
		//   key:
		//     The key of the element to get or set.
		//
		// Returns:
		//     The element with the specified key.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     key is null.
		//
		//   System.Collections.Generic.KeyNotFoundException:
		//     The property is retrieved and key is not found.
		//
		//   System.NotSupportedException:
		//     The property is set and the System.Collections.Generic.IDictionary<TKey,TValue>
		//     is read-only.
		T this[string key] { get; set; }

		// Summary:
		//     Adds an element with the provided key and value to the System.Collections.Generic.IDictionary<TKey,TValue>.
		//
		// Parameters:
		//   key:
		//     The object to use as the key of the element to add.
		//
		//   value:
		//     The object to use as the value of the element to add.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     key is null.
		//
		//   System.ArgumentException:
		//     An element with the same key already exists in the System.Collections.Generic.IDictionary<TKey,TValue>.
		//
		//   System.NotSupportedException:
		//     The System.Collections.Generic.IDictionary<TKey,TValue> is read-only.
		void Add(string key, T value);
		//
		// Summary:
		//     Determines whether the System.Collections.Generic.IDictionary<TKey,TValue>
		//     contains an element with the specified key.
		//
		// Parameters:
		//   key:
		//     The key to locate in the System.Collections.Generic.IDictionary<TKey,TValue>.
		//
		// Returns:
		//     true if the System.Collections.Generic.IDictionary<TKey,TValue> contains
		//     an element with the key; otherwise, false.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     key is null.
		bool ContainsKey(string key);
		//
		// Summary:
		//     Removes the element with the specified key from the System.Collections.Generic.IDictionary<TKey,TValue>.
		//
		// Parameters:
		//   key:
		//     The key of the element to remove.
		//
		// Returns:
		//     true if the element is successfully removed; otherwise, false. This method
		//     also returns false if key was not found in the original System.Collections.Generic.IDictionary<TKey,TValue>.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     key is null.
		//
		//   System.NotSupportedException:
		//     The System.Collections.Generic.IDictionary<TKey,TValue> is read-only.
		bool Remove(string key);
		//
		// Summary:
		//     Gets the value associated with the specified key.
		//
		// Parameters:
		//   key:
		//     The key whose value to get.
		//
		//   value:
		//     When this method returns, the value associated with the specified key, if
		//     the key is found; otherwise, the default value for the type of the value
		//     parameter. This parameter is passed uninitialized.
		//
		// Returns:
		//     true if the object that implements System.Collections.Generic.IDictionary<TKey,TValue>
		//     contains an element with the specified key; otherwise, false.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     key is null.
		bool TryGetValue(string key, out T value);
	}

	public class DictionaryList<T> : IDictionaryList<T> where T : class, INameable
	{
		private List<T> inner = new List<T>();

		public IEnumerable<T> Subset(int skip, int take)
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
			get
			{
				return inner[index];
			}
			set
			{
				inner[index] = value;
			}
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
			get
			{
				return inner.Count;
			}
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

		#region IDictionary<string,T> Members

		public void Add(string key, T value)
		{
			EnsureName(key, value);

			inner.Add(value);
		}

		public bool ContainsKey(string key)
		{
			return inner.Any(i => i.Name == key);
		}

		public ICollection<string> Keys
		{
			get { return inner.Select(i => i.Name).ToList(); }
		}

		public bool Remove(string key)
		{
			var index = inner.FindIndex(i => i.Name == key);
			if (index >= 0)
				inner.RemoveAt(index);
			return index >= 0;
		}

		public bool TryGetValue(string key, out T value)
		{
			value = inner.FirstOrDefault(i => i.Name == key);
			return value != null;
		}

		public ICollection<T> Values
		{
			get { return inner.ToList(); }
		}

		public T this[string key]
		{
			get
			{
				return inner.FirstOrDefault(i => i.Name == key);
			}
			set
			{
				EnsureName(key, value);

				int index = inner.FindIndex(i => i.Name == key);
				if (index < 0)
					Add(key, value);
				else
					inner[index] = value;
			}
		}

		#endregion

		#region IList Members

		int IList.Add(object value)
		{
			Add(value as T);
			return IndexOf(value as T);
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

		void IList.Remove(object value)
		{
			Remove(value as T);
		}

		object IList.this[int index]
		{
			get { return this[index]; }
			set { this[index] = value as T; }
		}

		#endregion

		private static void EnsureName(string key, T value)
		{
			if (value.Name != key)
				throw new InvalidOperationException("Cannot add value with differnet name (" + key + " != " + value.Name + ")");
		}
	}

}

namespace N2.Persistence.NH
{
	public class DictionaryListFactory<T> : IUserCollectionType where T : class, INameable
	{
		public DictionaryListFactory()
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
			return new DictionaryList<T>();
		}

		public IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister)
		{
			return new PersistentDictionaryList<T>(session);
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
			return new PersistentDictionaryList<T>(session, (IList<T>)collection);
		}

		#endregion
	}

	public class PersistentDictionaryList<T> : PersistentGenericBag<T>, IDictionaryList<T> where T : class, INameable
	{
		public PersistentDictionaryList(ISessionImplementor session)
			: base(session)
		{
			this.sessionImplementor = session;
		}

		public PersistentDictionaryList(ISessionImplementor session, IList<T> collection)
			: base(session, collection)
		{
			this.sessionImplementor = session;
		}

		private ICollectionPersister collectionPersister = null;
		public PersistentDictionaryList<T> CollectionPersister(ICollectionPersister collectionPersister)
		{
			this.collectionPersister = collectionPersister;
			return this;
		}

		protected ISessionImplementor sessionImplementor = null;

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

		#region IDictionary<string,T> Members

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

		public T this[string key]
		{
			get
			{
				return List.FirstOrDefault(i => i.Name == key);
			}
			set
			{
				EnsureName(key, value);

				var result = List.Select((item, index) => new { item, index }).FirstOrDefault(i => i.item.Name == key);
				if (result == null)
					Add(key, value);
				else
					this[result.index] = value;
			}
		}

		#endregion

		private static void EnsureName(string key, T value)
		{
			if (value.Name != key)
				throw new InvalidOperationException("Cannot add value with differnet name (" + key + " != " + value.Name + ")");
		}
	}
}
