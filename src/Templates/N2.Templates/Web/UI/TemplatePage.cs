using System;

namespace N2.Templates.Web.UI
{
	public class TemplatePage<TPage> : N2.Web.UI.Page<TPage> 
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
