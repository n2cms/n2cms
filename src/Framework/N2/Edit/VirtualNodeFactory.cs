using System.Linq;
using System.Collections.Generic;
using N2.Engine;

namespace N2.Edit
{
    public class ImmutableList<T> : IEnumerable<T>
    {
        List<T> items;

        public ImmutableList()
        {
            items = new List<T>();
        }

        public ImmutableList(IEnumerable<T> initialItems)
        {
            this.items = initialItems.ToList();
        }

        #region ICollection<T> Members

        public void Add(T item)
        {
            items = items.Union(new[] { item }).ToList();
        }

        public void Clear()
        {
            items = new List<T>();
        }

        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        public int Count
        {
            get { return items.Count; }
        }

        public bool Remove(T item)
        {
            int initialCount = Count;
            items = items.Except(new[] { item }).ToList();

            return initialCount > Count;
        }

        #endregion

        public void Reset(IEnumerable<T> replacementItems)
        {
            items = replacementItems.ToList();
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

    [Service]
    public class VirtualNodeFactory : INodeProvider
    {
        ImmutableList<INodeProvider> providers = new ImmutableList<INodeProvider>();

        //INodeProvider[] providers = new INodeProvider[0];

        public virtual ContentItem Get(string path)
        {
            foreach (var provider in providers)
            {
                ContentItem item = provider.Get(path);
                if (item != null)
                    return item;
            }

            return null;
        }

        public virtual IEnumerable<ContentItem> GetChildren(string path)
        {
            foreach (var provider in providers)
            {
                foreach (var item in provider.GetChildren(path))
                    yield return item;
            }
        }

        public virtual void Register(INodeProvider provider)
        {
            providers.Add(provider);
        }

        public virtual void Unregister(INodeProvider provider)
        {
            providers.Remove(provider);
        }
    }
}
