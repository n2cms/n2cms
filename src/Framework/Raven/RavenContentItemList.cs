using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Collections;
using N2.Persistence;
using Raven.Client;

namespace N2.Raven
{
	public class RavenContentItemList<T> : IContentItemList<T> where T : ContentItem
	{
		private List<T> inner = null;

		public RavenContentItemList(int enclosingItemID, IDocumentSession session)
		{
			this.EnclosingItemID = enclosingItemID;
			this.Session = session;
		}

		public int EnclosingItemID { get; set; }

		protected List<T> Inner
		{
			get
			{
				try
				{
					return inner
						?? (inner = Session.Query<T>()
						.Where(ci => ci.ParentID == EnclosingItemID).ToList());
				}
				catch (Exception)
				{
					
					throw;
				}
			}
		}

		public int IndexOf(T item)
		{
			return Inner.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			Inner.Insert(index, item);
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
			throw new NotImplementedException();
		}

		public int Count
		{
			get { return Inner.Count; }
		}

		public bool IsReadOnly
		{
			get { throw new NotImplementedException(); }
		}

		public bool Remove(T item)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return Inner.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Inner.GetEnumerator();
		}

		public T FindNamed(string name)
		{
			if (IsLoaded)
				return Inner.FirstOrDefault(ci => string.Equals(name, ci.Name, StringComparison.InvariantCultureIgnoreCase));

			return Session.Query<T>().Where(ci => ci.ParentID == EnclosingItemID)
				.Where(ci => ci.Name == name)
				.FirstOrDefault();
		}

		private bool IsLoaded
		{
			get { return inner != null; }
		}

		public ICollection<string> Keys
		{
			get { throw new NotImplementedException(); }
		}

		public ICollection<T> Values
		{
			get { throw new NotImplementedException(); }
		}

		public T this[string name]
		{
			get
			{
				return Inner.FirstOrDefault(i => string.Equals(i.Name, name, StringComparison.InvariantCultureIgnoreCase));
			}
			set
			{
				var index = Inner.FindIndex(i => string.Equals(i.Name, name, StringComparison.InvariantCultureIgnoreCase));
				if (index >= 0)
					Inner[index] = value;
				else
					Inner.Add(value);
			}
		}

		public void Add(string key, T value)
		{
			throw new NotImplementedException();
		}

		public bool ContainsKey(string key)
		{
			throw new NotImplementedException();
		}

		public bool Remove(string key)
		{
			throw new NotImplementedException();
		}

		public bool TryGetValue(string key, out T value)
		{
			throw new NotImplementedException();
		}

		public IQueryable<T> FindRange(int skip, int take)
		{
			throw new NotImplementedException();
		}

		public IQueryable<T> Query()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<T> FindNavigatablePages()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<T> FindPages()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<T> FindParts()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<T> FindParts(string zoneName)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<string> FindZoneNames()
		{
			throw new NotImplementedException();
		}

		public IDocumentSession Session { get; set; }
	}
}
