using System;
using N2.Web.UI;

namespace N2.Templates.Web.UI
{
	public class TemplatePage<TPage> : ContentPage<TPage> 
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
