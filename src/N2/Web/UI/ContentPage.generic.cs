using System;
using System.Web;
using N2.Engine;

namespace N2.Web.UI
{
    /// <summary>
    /// Page base class that provides easy access to the current page item.
    /// </summary>
    /// <typeparam name="TPage">The type of content item served by the page inheriting this class.</typeparam>
    public abstract class ContentPage<TPage> : System.Web.UI.Page, IItemContainer, IContentTemplate
        where TPage : N2.ContentItem
    {
		private TPage currentPage = null;
		private IEngine engine = null;

		/// <summary>Gets the current CMS Engine.</summary>
		public IEngine Engine
		{
			get { return engine ?? (engine = N2.Context.Current); }
			set { engine = value; }
		}

		/// <summary>Gets the content item associated with this page.</summary>
        public virtual TPage CurrentPage
        {
            get { return currentPage ?? (currentPage = ItemUtility.EnsureType<TPage>(N2.Context.CurrentPage)); }
			set { currentPage = value; }
        }

		/// <summary>Gets the content item associated with this page. This is most likely the same as CurrentPage unles you're in a user control dynamically added as a part.</summary>
		public virtual TPage CurrentItem
		{
			get { return CurrentPage; }
		}

        protected virtual bool AllowOutputCache
        {
            get { return true; }
        }

        protected override void OnInit(EventArgs e)
        {
            if (AllowOutputCache && (User == null || !User.Identity.IsAuthenticated))
            {
                ICacheManager cacheManager = Engine.Resolve<ICacheManager>();
                if (cacheManager.Enabled)
                {
                    InitOutputCache(cacheManager.GetOutputCacheParameters());
                    cacheManager.AddCacheInvalidation(this);
                }
            }
            
            base.OnInit(e);
        }

		#region IContentTemplate Members

		ContentItem IItemContainer.CurrentItem
		{
			get { return CurrentPage; }
		}

		ContentItem IContentTemplate.CurrentItem
		{
			get { return CurrentPage; }
			set { CurrentPage = ItemUtility.EnsureType<TPage>(value); }
		}
		#endregion
	}
}
