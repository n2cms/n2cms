using System;
using System.Web.Caching;
using System.Web.UI;
using N2.Web.Caching;
using System.Web.Configuration;
using N2.Templates.Configuration;

namespace N2.Templates.Web.UI
{
	public class TemplatePage<TPage> : N2.Web.UI.Page<TPage> 
		where TPage: Items.AbstractPage
	{
		public override string ID
		{
			get { return base.ID ?? "P"; }
		}

        protected virtual OutputCacheParameters OutputCacheParameters
        {
            get { return TemplatesSection.DefaultOutputCacheParameters; }
        }

		protected override void OnPreInit(EventArgs e)
		{
            if (OutputCacheParameters.Enabled)
            {
                Response.AddCacheDependency(new ContentCacheDependency(Engine.Persister));
                InitOutputCache(OutputCacheParameters);
            }

            Engine.Resolve<IPageModifierContainer>().Modify(this);

			base.OnPreInit(e);
		}
	}
}
