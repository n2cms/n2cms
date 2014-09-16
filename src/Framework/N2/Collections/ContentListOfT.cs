using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace N2.Collections
{
    internal sealed class CollectionDebugView<T>
    {
        private ICollection<T> collection;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] array = new T[this.collection.Count];
                this.collection.CopyTo(array, 0);
                return array;
            }
        }

        public CollectionDebugView(ICollection<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            this.collection = collection;
        }
    }

    /// <summary>
    /// A list of items that have a name with dictionary-like semantics.
    /// </summary>
    /// <typeparam name="T">The type of item to list.</typeparam>
    [DebuggerDisplay("ContentList, Count = {inner.Count}")]
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    public class ContentList<T> : IContentList<T>, IList where T : class, INameable
    {
        public ContentList()
        {
            inner = new List<T>();
        }

        public ContentList(IEnumerable<T> inner)
        {
            this.lazyInner = () => inner;
        }

        public ContentList(Func<IEnumerable<T>> innerFactory)
        {
            this.lazyInner = innerFactory;
        }

        private List<T> inner;
        private Func<IEnumerable<T>> lazyInner;

        protected List<T> Inner
        {
            get { return inner ?? (inner = lazyInner().ToList()); }
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
            return Inner.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            Inner.Insert(Math.Max(0,index), item);
        }

        public void RemoveAt(int index)
        {
            Inner.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return Inner[index];
            }
            set
            {
                Inner[index] = value;
            }
        }

        public void Add(T item)
        {
            Inner.Add(item);
        }

        public void Clear()
        {
            Inner.Clear();
        }

        public bool Contains(T item)
        {
            return Inner.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Inner.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return Inner.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            return Inner.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Inner.GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            T[] arr = new T[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                arr[i] = (T)array.GetValue(i);
            }

            Inner.CopyTo(arr, index);
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

        /// <summary>Adds an element with the provided key and value to the list.</summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        public void Add(string key, T value)
        {
            EnsureName(key, value);

            Inner.Add(value);
        }

        /// <summary>Determines whether the list contains an element with the specified key.</summary>
        /// <param name="key">The key to locate in the list.</param>
        /// <returns>true if the System.Collections.Generic.IDictionary<TKey,TValue> contains an element with the key; otherwise, false.</returns>
        public bool ContainsKey(string key)
        {
            return Inner.Any(i => i.Name == key);
        }

        /// <summary>Gets an System.Collections.Generic.ICollection<T> containing the keys of the System.Collections.Generic.IDictionary<TKey,TValue>.</summary>
        public ICollection<string> Keys
        {
            get { return Inner.Select(i => i.Name).ToList(); }
        }

        /// <summary>Removes the element with the specified key from the System.Collections.Generic.IDictionary<TKey,TValue>.</summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>true if the element is successfully removed; otherwise, false. This method also returns false if key was not found in the original list.</returns>
        public bool Remove(string key)
        {
            var index = Inner.FindIndex(i => i.Name == key);
            if (index >= 0)
                Inner.RemoveAt(index);
            return index >= 0;
        }

        /// <summary>Gets the value associated with the specified key.</summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the list contains an element with the specified key; otherwise, false.</returns>
        public bool TryGetValue(string key, out T value)
        {
            value = Inner.FirstOrDefault(i => i.Name == key);
            return value != null;
        }

        /// <summary>Gets an System.Collections.Generic.ICollection<T> containing the values in the System.Collections.Generic.IDictionary<TKey,TValue>.</summary>
        public ICollection<T> Values
        {
            get { return Inner.ToList(); }
        }

        /// <summary>Gets or sets the element with the specified key.</summary>
        /// <param name="name">The key of the element to get or set.</param>
        /// <returns>The element with the specified key.</returns>
        public T this[string name]
        {
            get
            {
                return FindNamed(name);
            }
            set
            {
                EnsureName(name, value);

                int index = Inner.FindIndex(i => i.Name == name);
                if (index < 0)
                    Add(name, value);
                else
                    Inner[index] = value;
            }
        }

        /// <summary>Finds an item with the given name.</summary>
        /// <param name="name">The name of the item to find.</param>
        /// <returns>The item with the given name or null if no item was found.</returns>
        public virtual T FindNamed(string name)
        {
            return Inner.FirstOrDefault(i => string.Equals(i.Name, name, StringComparison.InvariantCultureIgnoreCase));
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

        public IQueryable<T> FindRange(int skip, int take)
        {
            return Inner.Skip(skip).Take(take).AsQueryable();
        }

        #endregion

        #region IQueryableList<T> Members

        public virtual IQueryable<T> Query()
        {
            return Inner.AsQueryable<T>();
        }

        #endregion

        public IContentList<T> Clone()
        {
            return new ContentList<T>(Inner.ToList());
        }

        public bool WasInitialized
        {
            get { return true; }
        }
    }
}
