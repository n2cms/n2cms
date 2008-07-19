using System;
using System.Web.Caching;
using System.Web.UI;
using N2.Web.Caching;
using System.Web.Configuration;
using N2.Templates.Configuration;
using System.Web;

namespace N2.Templates.Web.UI
{
	public class TemplatePage<TPage> : N2.Web.UI.Page<TPage> 
		where TPage: Items.AbstractPage
	{
		public override string ID
		{
			get { return base.ID ?? "P"; }
		}

        private OutputCacheParameters cacheParameters = null;
        protected virtual OutputCacheParameters CacheParameters
        {
            get { return cacheParameters ?? (cacheParameters = TemplatesSection.OutputCacheParameters); }
        }
        protected virtual bool OutputCacheEnabled
        {
            get { return TemplatesSection.OutputCacheEnabled; }
        }
        public static HttpCacheValidateHandler CacheThisRequest = delegate(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            if (context.User.Identity.IsAuthenticated)
                validationStatus = HttpValidationStatus.IgnoreThisRequest;
        };

		protected override void OnPreInit(EventArgs e)
		{
            if (OutputCacheEnabled && !User.Identity.IsAuthenticated)
            {
                OutputCacheParameters parameters = CacheParameters;
                if (CurrentPage != null && CurrentPage.Expires.HasValue)
                {
                    DateTime expires = CurrentPage.Expires.Value;
                    if (expires > DateTime.Now && expires < DateTime.Now.AddSeconds(parameters.Duration))
                    {
                        parameters.Duration = (int)expires.Subtract(DateTime.Now).TotalSeconds;
                    }
                }
                InitOutputCache(parameters);
                Response.AddCacheDependency(new ContentCacheDependency(Engine.Persister));
                Response.Cache.AddValidationCallback(CacheThisRequest, null);
            }

            Engine.Resolve<IPageModifierContainer>().Modify(this);

			base.OnPreInit(e);
		}
	}
}
