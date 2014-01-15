using System;
using System.Collections.Generic;

namespace N2.Collections
{
    /// <summary>
    /// Enumerates child items recursively to find items of a certain type, or 
    /// implementing a certain interface.
    /// </summary>
    public class RecursiveFinder
    {
        /// <summary>Gets child items of a certain type within a certain depth.</summary>
        /// <typeparam name="T">Type of item to find</typeparam>
        /// <param name="root">The initial item.</param>
        /// <param name="recursionDepth">The maximum recursion depth.</param>
        /// <returns>An enumeration of items of the given type.</returns>
        public IEnumerable<T> Find<T>(ContentItem root, int recursionDepth)
            where T : class
        {
            List<T> items = new List<T>();
            AppendRecursive(root, items, recursionDepth);
            return items;
        }

        /// <summary>Gets child items of a certain type within a certain depth.</summary>
        /// <typeparam name="T">Type of item to find</typeparam>
        /// <param name="root">The initial item.</param>
        /// <param name="recursionDepth">The maximum recursion depth.</param>
        /// <param name="except">Types to ignore (do not recurse into).</param>
        /// <returns>An enumeration of items of the given type.</returns>
        public IEnumerable<T> Find<T>(ContentItem root, int recursionDepth, params Type[] except)
            where T : class
        {
            List<T> items = new List<T>();
            AppendRecursive(root, items, recursionDepth, except);
            return items;
        }

        private void AppendRecursive<T>(ContentItem item, List<T> items, int depth, params Type[] except)
            where T : class
        {
            if (item is T)
                items.Add(item as T);

            if (depth <= 0)
                return;

            foreach (ContentItem child in item.Children)
            {
                if (Is(child, except))
                    continue;

                AppendRecursive(child, items, depth - 1);
            }
        }

        private static bool Is(ContentItem child, Type[] except)
        {
            foreach (Type t in except)
                if (t.IsAssignableFrom(child.GetContentType()))
                    return true;
            return false;
        }
    }
}
