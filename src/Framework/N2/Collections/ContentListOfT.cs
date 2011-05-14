using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace N2.Collections
{
	public class ContentList<T> : IContentList<T>, IList where T : class, INameable
	{
		private List<T> inner = new List<T>();

		protected List<T> Inner
		{
			get { return inner; }
			set { inner = value; }
		}

		private static void EnsureName(string key, T value)
		{
			if (value.Name != key)
				throw new InvalidOperationException("Cannot add value with differnet name (" + key + " != " + value.Name + ")");
		}

		#region IList Members

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

		#endregion

		#region INamedList<T> Members

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

		public T this[string name]
		{
			get
			{
				return FindNamed(name);
			}
			set
			{
				EnsureName(name, value);

				int index = inner.FindIndex(i => i.Name == name);
				if (index < 0)
					Add(name, value);
				else
					inner[index] = value;
			}
		}

		public T FindNamed(string name)
		{
			return inner.FirstOrDefault(i => string.Equals(i.Name, name, StringComparison.InvariantCultureIgnoreCase));
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

		#region IPageableList<T> Members

		public IList<T> FindRange(int skip, int take)
		{
			return inner.Skip(skip).Take(take).ToList();
		}

		#endregion
	}
}
