using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web;
using N2.Configuration;
using N2.Persistence;

namespace N2.Web.UI
{
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

        public virtual void AddCacheInvalidation(System.Web.UI.Page page)
        {
            page.Response.AddCacheDependency(new ContentCacheDependency(persister));
            page.Response.Cache.AddValidationCallback(ValidateCacheRequest, null);
        }

        public virtual OutputCacheParameters GetOutputCacheParameters()
        {
            OutputCacheParameters parameters = new OutputCacheParameters();
            parameters.CacheProfile = cacheProfile;
            parameters.Duration = duration;
            if (context.CurrentPage != null && context.CurrentPage.Expires.HasValue)
            {
                DateTime expires = context.CurrentPage.Expires.Value;
                if (expires > DateTime.Now && expires < DateTime.Now.AddSeconds(parameters.Duration))
                {
                    parameters.Duration = (int)expires.Subtract(DateTime.Now).TotalSeconds;
                }
            }
            parameters.Enabled = enabled;
            parameters.Location = OutputCacheLocation.Server;
            //parameters.NoStore = outputCacheParameters.NoStore;
            //parameters.SqlDependency = outputCacheParameters.SqlDependency;
            //parameters.VaryByContentEncoding = outputCacheParameters.VaryByContentEncoding;
            //parameters.VaryByControl = parameters.VaryByControl;
            //parameters.VaryByCustom = parameters.VaryByCustom;
            //parameters.VaryByHeader = parameters.VaryByHeader;
            parameters.VaryByParam = parameters.VaryByParam;
            return parameters;
        }
    }
}
