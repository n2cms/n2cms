using System;
using System.Web.Caching;
using System.Web.UI;
using System.Web.Configuration;
using N2.Templates.Configuration;
using System.Web;
using N2.Web.UI;

namespace N2.Templates.Web.UI
{
	public class TemplatePage<TPage> : N2.Web.UI.ContentPage<TPage> 
		where TPage: Items.AbstractPage
	{
		public override string ID
		{
			get { return base.ID ?? "P"; }
		}

		protected override void OnPreInit(EventArgs e)
		{
            Engine.Resolve<IPageModifierContainer>().Modify(this);

			base.OnPreInit(e);
		}
	}
}
