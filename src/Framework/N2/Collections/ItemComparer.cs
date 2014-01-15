using System.Collections;

namespace N2.Collections
{
    /// <summary>
    /// Compares two content items based on a detail. This class can compare 
    /// classes given a expression.
    /// </summary>
    public class ItemComparer : ItemComparer<ContentItem>, IComparer
    {
        #region Constructors
        /// <summary>Creates a new instance of the ItemComparer that sorts on the item's sort order.</summary>
        public ItemComparer() 
            : this("SortOrder")
        {
        }

        /// <summary>Creates a new instance of the ItemComparer that sorts using a custom sort expression.</summary>
        /// <param name="sortExpression">The name of the property to sort on. DESC can be appended to the string to reverse the sort order.</param>
        public ItemComparer(string sortExpression) 
            : base(sortExpression)
        {
        }

        /// <summary>Creates a new instance of the ItemComparer that sorts using sort property and direction.</summary>
        /// <param name="detailToCompare">The name of the property to sort on.</param>
        /// <param name="inverse">Wether the comparison should be "inverse", i.e. make Z less than A.</param>
        public ItemComparer(string detailToCompare, bool inverse)
            : base(detailToCompare, inverse)
        {
        }
        #endregion
    }
}
