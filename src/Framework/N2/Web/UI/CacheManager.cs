using System;
using System.Web;
using System.Web.UI;
using N2.Configuration;
using N2.Engine;
using N2.Persistence;

namespace N2.Web.UI
{
    [Service(typeof(ICacheManager))]
    public class CacheManager : N2.Web.UI.ICacheManager
    {
        IWebContext context;
        IPersister persister;

        bool enabled = false;
        string varyByParam = "*";
        string cacheProfile = "";
        int duration = 60;

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
            cacheProfile = config.OutputCache.CacheProfile;
            duration = config.OutputCache.Duration;
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

        public virtual void AddCacheInvalidation(HttpResponse response)
        {
            response.AddCacheDependency(new ContentCacheDependency(persister));
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
            parameters.Enabled = enabled;
            parameters.Location = OutputCacheLocation.Server;
            //parameters.NoStore = NoStore;
            //parameters.SqlDependency = SqlDependency;
            //parameters.VaryByContentEncoding = VaryByContentEncoding;
            //parameters.VaryByControl = VaryByControl;
            //parameters.VaryByCustom = VaryByCustom;
            //parameters.VaryByHeader = VaryByHeader;
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
