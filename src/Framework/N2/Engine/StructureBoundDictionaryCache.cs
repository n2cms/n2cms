using System;
using System.Collections.Generic;
using N2.Persistence;

namespace N2.Engine
{
    /// <summary>
    /// Caches a dynamically expandable dictionary that is expired when an item is moved or deleted.
    /// </summary>
    /// <typeparam name="K">The type of key for the dictionary.</typeparam>
    /// <typeparam name="V">The type of values stored in the dictionary.</typeparam>
    [Service]
    public class StructureBoundDictionaryCache<K, V> : StructureBoundCache<IDictionary<K, V>>
    {
        public StructureBoundDictionaryCache(IPersister persister) 
            : base(persister)
        {
        }

        /// <summary>Gets the value from the ditionary or uses the provided factory method to create it.</summary>
        /// <param name="key"></param>
        /// <param name="valueFactoryMethod"></param>
        /// <returns></returns>
        public virtual V GetValue(K key, Func<K, V> valueFactoryMethod)
        {
            var dictionary = base.GetValue(() => new Dictionary<K, V>());

            V value = default(V);
            lock (this)
            {
                if (!dictionary.TryGetValue(key, out value))
                {
                    value = valueFactoryMethod(key);
                    dictionary[key] = value;
                    if (DictionaryValueCreated != null)
                        DictionaryValueCreated.Invoke(this, new ValueEventArgs<V> { Value = value });
                }
            }

            return value;
        }

        public event EventHandler<ValueEventArgs<V>> DictionaryValueCreated;
    }
}
