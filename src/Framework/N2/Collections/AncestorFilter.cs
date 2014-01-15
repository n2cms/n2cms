using System;
using System.Collections.Generic;

namespace N2.Collections
{
    /// <summary>
    /// Filters items not belonging to a certain parent item.
    /// </summary>
    [Obsolete("Use AncestorFilter")]
    public class ParentFilter : AncestorFilter
    {
        public ParentFilter(int parentID)
            : base(parentID)
        {
        }

        public ParentFilter(ContentItem parent)
            : base(parent)
        {
        }
    }

    /// <summary>
    /// Filters items not belonging to a certain page hierarchy.
    /// </summary>
    public class AncestorFilter : ItemFilter
    {
        private int? parentID;
        private bool orSelf;

        public AncestorFilter(int parentID)
        {
            this.parentID = parentID;
        }

        public AncestorFilter(ContentItem parent)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            this.parentID = parent.ID;
        }

        public AncestorFilter(int parentID, bool orSelf)
            : this(parentID)
        {
            this.orSelf = orSelf;
        }

        public AncestorFilter(ContentItem parent, bool orSelf)
            : this(parent)
        {
            this.orSelf = orSelf;
        }

        public override bool Match(ContentItem item)
        {
            return (orSelf && item.ID == parentID) || (item.AncestralTrail ?? Utility.GetTrail(item.Parent)).Contains("/" + parentID + "/");
        }

        #region Static Methods
        public static void Filter(IList<ContentItem> items, ContentItem parent)
        {
            Filter(items, new AncestorFilter(parent));
        }
        public static void Filter(IList<ContentItem> items, int parentID)
        {
            Filter(items, new AncestorFilter(parentID));
        }
        #endregion

        public override string ToString()
        {
            return "Ancestor.ID=" + parentID;
        }
    }
}
