using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Behaviors
{
    public class BehaviorContext
    {
        public BehaviorContext()
        {
            UnsavedItems = new List<ContentItem>();
        }

        /// <summary>The item being affected by the current operation.</summary>
        public ContentItem AffectedItem { get; set; }

        /// <summary>Items to be saved in conjunction with the affected item.</summary>
        public ICollection<ContentItem> UnsavedItems { get; set; }

        public string Action { get; set; }

        public ContentItem Parent { get; set; }
    }
}
