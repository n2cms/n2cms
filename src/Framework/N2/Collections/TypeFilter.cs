using System;
using System.Linq;
using System.Collections.Generic;

namespace N2.Collections
{
    /// <summary>
    /// Filters based on item type.
    /// </summary>
    public class TypeFilter : ItemFilter
    {
        #region Constructors
        /// <summary>Instantiates a type filter that will filter anythning but the supplied types (and subclasses).</summary>
        /// <param name="allowedTypes">Types that will match.</param>
        public TypeFilter(params Type[] allowedTypes)
        {
            this.allowedTypes = allowedTypes;
        }

        /// <summary>Instantiates a type filter that will filter anythning but the supplied types (and subclasses).</summary>
        /// <param name="allowedTypes">Types that will match.</param>
        public TypeFilter(IEnumerable<Type> allowedTypes)
        {
            this.allowedTypes = allowedTypes;
        }

        /// <summary>Instantiats a type filter that will filter based on the supplied types. The types will be kept or filtered out depending on the inverse parameter.</summary>
        /// <param name="inverse">True means that supplied types will be removed when filtering.</param>
        /// <param name="allowedTypes">The types that will match or the inverse based on the inverse parameter.</param>
        [Obsolete("Wrap i an'inverse filter instead.")]
        public TypeFilter(bool inverse, params Type[] allowedTypes)
            : this(allowedTypes)
        {
            this.inverse = inverse;
        } 
        #endregion

        #region Private Members
        private IEnumerable<Type> allowedTypes;
        private bool inverse = false; 
        #endregion

        #region Properties
        /// <summary>Gets or sets whether matching types should be removed instead beeing kept.</summary>
        public bool Inverse
        {
            get { return inverse; }
            set { inverse = value; }
        }

        /// <summary>Gets or sets types that should match and thus are kept when applying this filter. This behaviour can be inversed by setting the Inverse property to true.</summary>
        public IEnumerable<Type> AllowedTypes
        {
            get { return allowedTypes; }
            set { allowedTypes = value; }
        } 
        #endregion

        #region Methods
        public override bool Match(ContentItem item)
        {
            Type itemType = item.GetContentType();
            foreach (Type t in allowedTypes)
                if (t.IsAssignableFrom(itemType))
                    return !inverse;
            return inverse;
        } 
        #endregion

        public static void Filter(IList<ContentItem> items, params Type[] allowedTypes)
        {
            Filter(items, new TypeFilter(allowedTypes));
        }

        /// <summary>A shorthand for type filter creation.</summary>
        /// <typeparam name="T">The type to filter.</typeparam>
        /// <returns>A new type filter.</returns>
        public static TypeFilter Of<T>()
        {
            return new TypeFilter(typeof (T));
        }

        public override string ToString()
        {
            return "Type is" + (Inverse ? "n't " : " ") + string.Join(",", AllowedTypes.Select(t => t.Name).ToArray());
        }
    }
}
