using System;
using System.Web;
using N2.Engine;
using N2.Web.Mvc;

namespace N2.Web.UI
{
    /// <summary>
    /// Page base class that provides easy access to the current page item.
    /// </summary>
    /// <typeparam name="TPage">The type of content item served by the page inheriting this class.</typeparam>
    public class ContentPage<TPage> : System.Web.UI.Page, IItemContainer, IContentTemplate, IProvider<IEngine>
        where TPage : N2.ContentItem
    {
		private TPage currentPage = null;
		private IEngine engine = null;
		private FormsContentHelper content = null;

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

		public FormsContentHelper Content
		{
			get { return content ?? (content = new FormsContentHelper(() => Engine, () => new PathData { CurrentPage = CurrentPage, CurrentItem = CurrentItem })); }
			set { content = value; }
		}

        protected virtual bool AllowOutputCache
        {
            get { return true; }
        }

		protected override void OnPreInit(EventArgs e)
		{
			ApplyConcerns(CurrentItem);

			base.OnPreInit(e);
		}

    	protected override void OnInit(EventArgs e)
        {
            if (AllowOutputCache && (User == null || !User.Identity.IsAuthenticated))
            {
                ApplyCaching();
            }
            
            base.OnInit(e);
        }

		/// <summary>Applies all <see cref="ContentPageConcern"/> added to <see cref="IServiceContainer"/>.</summary>
		/// <param name="item">The current item.</param>
		protected virtual void ApplyConcerns(ContentItem item)
		{
			if (HttpContext.Current == null)
				return;

			foreach (var concern in Engine.Container.ResolveAll<ContentPageConcern>())
				concern.OnPreInit(this, item);

			foreach (IContentPageConcern concern in GetType().GetCustomAttributes(typeof(IContentPageConcern), true))
				concern.OnPreInit(this, item);
		}

		/// <summary>Applies configured cache parameters to this page.</summary>
		protected virtual void ApplyCaching()
    	{
    		ICacheManager cacheManager = Engine.Resolve<ICacheManager>();
    		if (cacheManager.Enabled)
    		{
    			InitOutputCache(cacheManager.GetOutputCacheParameters());
    			cacheManager.AddCacheInvalidation(Response);
    		}
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

		#region IProvider<IEngine> Members

		IEngine IProvider<IEngine>.Get()
		{
			return Engine;
		}

		System.Collections.Generic.IEnumerable<IEngine> IProvider<IEngine>.GetAll()
		{
			return Engine.Container.ResolveAll<IEngine>();
		}

		#endregion
	}
}
