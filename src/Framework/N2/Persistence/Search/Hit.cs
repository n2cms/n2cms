using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Search
{
    public class LightweightHitData
    {
        public string Path { get; set; }
        public Security.Permission AlteredPermissions { get; set; }
        public IEnumerable<string> AuthorizedRoles { get; set; }
        public int ID { get; set; }
        public ContentState State { get; set; }
        public bool Visible { get; set; }
    }

    /// <summary>
    /// A single search result.
    /// </summary>
    public class Hit<T> where T : class
    {
        /// <summary>The content item found.</summary>
        public virtual T Content { get; set; }

        /// <summary>The search hit score greater than 0.</summary>
        public double Score { get; set; }

        // The hit title as stored in the index.
        public string Title { get; set; }

        // The hit url as stored in the index.
        public string Url { get; set; }
    }

    /// <summary>
    /// A single search result which is lazily resolved.
    /// </summary>
    public class LazyHit<T> : Hit<T> where T : class
    {
        public int ID { get; set; }

        /// <summary>The content item found.</summary>
        public override T Content 
        {
            get { return base.Content ?? (base.Content = ContentAccessor(ID)); }
            set { base.Content = value; }
        }

        /// <summary>Lazily resolves the content upon access.</summary>
        public virtual Func<int, T> ContentAccessor { get; set; }
    }
}
