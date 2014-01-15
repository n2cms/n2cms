using System;
using N2.Web;

namespace N2.Engine
{
    /// <summary>
    /// Convenience base class for content adapters. One instance 
    /// of the inheriting class is created per request upon usage.
    /// </summary>
    public abstract class AbstractContentAdapter : IComparable<AbstractContentAdapter>
    {
        private IEngine engine;
        
        public AbstractContentAdapter()
        {
            AdaptedType = typeof(ContentItem);
        }

        #region IContentAdapter Members

        /// <summary>The path associated with this controller instance.</summary>
        [Obsolete("Use provided parameters or resolve path using engine.", true)]
        public PathData Path 
        {
            get { return Engine.Resolve<IWebContext>().CurrentPath; }
        }

        /// <summary>The type adapted by this adapter instance.</summary>
        public Type AdaptedType { get; set; }

        /// <summary>The content engine requesting external control.</summary>
        public IEngine Engine
        {
            get { return engine ?? Context.Current; }
            set { engine = value; }
        }

        #endregion


        #region IComparable<AbstractContentAdapter> Members

        int IComparable<AbstractContentAdapter>.CompareTo(AbstractContentAdapter other)
        {
            int adaptedTypeComparisonResult = Utility.InheritanceDepth(other.AdaptedType).CompareTo(Utility.InheritanceDepth(AdaptedType));
            if (AdaptedType.IsInterface ^ other.AdaptedType.IsInterface)
                // if one of the items is an interface sort it before the class
                adaptedTypeComparisonResult = AdaptedType.IsInterface ? -1 : 1;
            return 2 * adaptedTypeComparisonResult // order by depth of adapted type first
                + Utility.InheritanceDepth(other.GetType()).CompareTo(Utility.InheritanceDepth(GetType())); // then by depth of adapter
        }

        #endregion
    }
}
