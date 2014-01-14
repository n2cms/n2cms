using System.Collections.Generic;

namespace N2.Collections
{
    /// <summary>
    /// A filter that removes duplicated by keeping track of already added 
    /// items and only matching new items.
    /// </summary>
    public class DuplicateFilter : ItemFilter
    {
        #region Private Fields
        private Dictionary<int, bool> existing = new Dictionary<int, bool>();   
        #endregion

        #region Public Methods
        public override bool Match(N2.ContentItem item)
        {
            if (existing.ContainsKey(item.ID))
                return false;

            existing.Add(item.ID, true);
            return true;
        }

        public void Clear()
        {
            existing.Clear();
        } 
        #endregion

        /// <summary>Removes duplicate items.</summary>
        /// <param name="items">The items whose duplicate items should be deleted.</param>
        public static void FilterDuplicates(IList<ContentItem> items)
        {
            ItemFilter.Filter(items, new DuplicateFilter());
        }

        #region IDisposable Members

        public override void Dispose()
        {
            Clear();
        }

        #endregion

        public override string ToString()
        {
            return "Distinct";
        }
    }
}
