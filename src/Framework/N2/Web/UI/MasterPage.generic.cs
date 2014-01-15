using N2.Engine;

namespace N2.Web.UI
{
    /// <summary>MasterPage base class providing easy access to current page item.</summary>
    /// <typeparam name="TPage">The type of content item for this masterpage</typeparam>
    public abstract class MasterPage<TPage> : System.Web.UI.MasterPage, IItemContainer 
        where TPage : ContentItem
    {
        private IEngine engine = null;

        /// <summary>Gets the current CMS Engine.</summary>
        public IEngine Engine
        {
            get { return engine ?? (engine = N2.Context.Current); }
            set { engine = value; }
        }

        public virtual TPage CurrentPage
        {
            get { return (N2.Context.CurrentPage ?? Content.Traverse.StartPage) as TPage; }
        }

        public virtual TPage CurrentItem
        {
            get { return CurrentPage; }
        }

        #region IItemContainer Members

        ContentItem IItemContainer.CurrentItem
        {
            get { return this.CurrentPage; }
        }

        #endregion
    }
}
