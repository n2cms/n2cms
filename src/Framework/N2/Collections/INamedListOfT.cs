using System.Collections.Generic;

namespace N2.Collections
{
    /// <summary>
    /// A list of items that have a name with dictionary-like semantics.
    /// </summary>
    /// <typeparam name="T">The type of item to list.</typeparam>
    public interface INamedList<T> where T : class, INameable
    {
        /// <summary>Finds an item with the given name.</summary>
        /// <param name="name">The name of the item to find.</param>
        /// <returns>The item with the given name or null if no item was found.</returns>
        T FindNamed(string name);

        /// <summary>Gets an System.Collections.Generic.ICollection<T> containing the keys of the System.Collections.Generic.IDictionary&gt;TKey,TValue&lt;.</summary>
        ICollection<string> Keys { get; }

        /// <summary>Gets an System.Collections.Generic.ICollection<T> containing the values in the System.Collections.Generic.IDictionary&gt;TKey,TValue&lt;.</summary>
        ICollection<T> Values { get; }

        /// <summary>Gets or sets the element with the specified key.</summary>
        /// <param name="name">The key of the element to get or set.</param>
        /// <returns>The element with the specified key.</returns>
        T this[string name] { get; set; }

        /// <summary>Adds an element with the provided key and value to the list.</summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        void Add(string key, T value);

        /// <summary>Determines whether the list contains an element with the specified key.</summary>
        /// <param name="key">The key to locate in the list.</param>
        /// <returns>true if the System.Collections.Generic.IDictionary&gt;TKey,TValue&lt; contains an element with the key; otherwise, false.</returns>
        bool ContainsKey(string key);

        /// <summary>Removes the element with the specified key from the System.Collections.Generic.IDictionary&gt;TKey,TValue&lt;.</summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>true if the element is successfully removed; otherwise, false. This method also returns false if key was not found in the original list.</returns>
        bool Remove(string key);

        /// <summary>Gets the value associated with the specified key.</summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the list contains an element with the specified key; otherwise, false.</returns>
        bool TryGetValue(string key, out T value);
    }
}
