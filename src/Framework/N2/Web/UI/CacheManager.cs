using System;
using System.Web;
using System.Web.UI;
using N2.Configuration;
using N2.Engine;
using N2.Persistence;
using N2.Definitions;

namespace N2.Web.UI
{
    [Service(typeof(ICacheManager))]
    public class CacheManager : N2.Web.UI.ICacheManager
    {
        IWebContext context;
        IPersister persister;

        bool enabled = false;
        string varyByParam = "*";
        string varyByHeader = "";
        string cacheProfile = "";
		string varyByCustom = "";
		int duration = 60;
		private OutputCacheInvalidationMode invalidationMode;

		public CacheManager(IWebContext context, IPersister persister)
        {
            this.context = context;
            this.persister = persister;
        }

        public CacheManager(IWebContext context, IPersister persister, HostSection config)
            : this(context, persister)
        {
            enabled = config.OutputCache.Enabled;
            varyByParam = config.OutputCache.VaryByParam;
            varyByHeader = config.OutputCache.VaryByHeader;
			varyByCustom = config.OutputCache.VaryByCustom;
            cacheProfile = config.OutputCache.CacheProfile;
            duration = config.OutputCache.Duration;
			invalidationMode = config.OutputCache.InvalidateOnChangesTo;
        }

        public bool Enabled
        {
            get { return enabled; }
        }

        public virtual void ValidateCacheRequest(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            if (context.User != null && context.User.Identity.IsAuthenticated)
                validationStatus = HttpValidationStatus.IgnoreThisRequest;
        }

		public virtual void AddCacheInvalidation(ContentItem item, HttpResponse response)
		{
			switch(invalidationMode)
			{
				case OutputCacheInvalidationMode.All:
					AddCacheDependency(response, new ContentCacheDependency(persister));
					break;
				case OutputCacheInvalidationMode.IgnoreChanges:
					AddCacheDependency(response, null);
					break;
				case OutputCacheInvalidationMode.Page:
					if (item == null) return;
					AddCacheDependency(response, new PageContentCacheDependency(persister, item.ID));
					break;
				case OutputCacheInvalidationMode.Site:
				case OutputCacheInvalidationMode.SiteSection:
					if (item == null) return;
					var ancestors = Find.EnumerateParents(item, null, includeSelf: true);
					ContentItem invalidateBelow = null;
					foreach (var ancestor in ancestors)
					{
						if (ancestor is IStartPage)
						{
							if (invalidationMode == OutputCacheInvalidationMode.Site)
							{
								invalidateBelow = ancestor;
							}
							break;
						}
						invalidateBelow = ancestor;
					}
					AddCacheDependency(response, new SectionContentCacheDependency(persister, invalidateBelow.GetTrail()));
					break;
				default:
					throw new NotSupportedException("Unsupported cache invalidation mode " + invalidationMode);
			}
		}

		[Obsolete]
		public virtual void AddCacheInvalidation(HttpResponse response)
        {
			AddCacheDependency(response, new ContentCacheDependency(persister));
        }

		protected virtual void AddCacheDependency(HttpResponse response, ContentCacheDependency dependency)
		{
			if (dependency != null)
				response.AddCacheDependency(dependency);
			response.Cache.AddValidationCallback(ValidateCacheRequest, null);
		}

        public virtual OutputCacheParameters GetOutputCacheParameters()
        {
            OutputCacheParameters parameters = new OutputCacheParameters();
            parameters.CacheProfile = cacheProfile;
            parameters.Duration = duration;
            if (context.CurrentPage != null && context.CurrentPage.Expires.HasValue)
            {
                DateTime expires = context.CurrentPage.Expires.Value;
                if (expires > N2.Utility.CurrentTime() && expires < N2.Utility.CurrentTime().AddSeconds(parameters.Duration))
                {
                    parameters.Duration = (int)expires.Subtract(N2.Utility.CurrentTime()).TotalSeconds;
                }
            }
            parameters.Enabled = Enabled;
            parameters.Location = OutputCacheLocation.Server;
			if (!string.IsNullOrEmpty(varyByHeader))
				parameters.VaryByHeader = varyByHeader;
			if (!string.IsNullOrEmpty(varyByCustom))
				parameters.VaryByCustom = varyByCustom;
			parameters.VaryByParam = varyByParam;
            return parameters;
        }
    }

    public static class CacheUtility
    {
        public static void InitOutputCache(ICacheManager cacheManager, HttpContext context)
        {
            if (!cacheManager.Enabled)
                return;

            OutputCacheParameters cacheSettings = cacheManager.GetOutputCacheParameters();

        }
    }















}
