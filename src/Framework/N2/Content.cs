using N2.Collections;
using N2.Web;
using N2.Persistence;

namespace N2
{
    /// <summary>
    /// Provides access to common functions.
    /// </summary>
    public static class Content
    {
        /// <summary>
        /// Provides access to filters applyable to content items.
        /// </summary>
        public static FilterHelper Is
        {
            get { return new FilterHelper(() => Context.Current); }
        }

        /// <summary>
        /// Simplifies traversing items in the content hierarchy.
        /// </summary>
        public static TraverseHelper Traverse
        {
            get { return new TraverseHelper(() => Context.Current, Is, PathGetter); }
        }

        /// <summary>
        /// Simplifies access to APIs related to search and querying.
        /// </summary>
        public static SearchHelper Search
        {
            get { return new SearchHelper(() => Context.Current); }
        }

        /// <summary>
        /// Simplifies access to the current page
        /// </summary>
        public static ContextHelper Current
        {
            get { return new ContextHelper(() => Context.Current, PathGetter); }
        }

        private static System.Func<PathData> PathGetter
        {
            get { return () => Context.Current.Resolve<IWebContext>().CurrentPath; }
        }
    }
}
