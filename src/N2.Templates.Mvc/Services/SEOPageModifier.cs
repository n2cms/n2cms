using Castle.Core;
using N2.Templates.Mvc.Web;
using N2.Plugin;
using N2.Web.UI;

namespace N2.Templates.Mvc.Services
{
	public class SEOPageModifier : IPageModifier, IStartable, IAutoStart
	{
		private readonly IPageModifierContainer modifierContainer;

		public SEOPageModifier(IPageModifierContainer modifierContainer)
		{
			this.modifierContainer = modifierContainer;
		}

		public void Modify<T>(ContentPage<T> page) where T : ContentItem
		{
			new TitleAndMetaTagApplyer(page, page.CurrentPage);
		}

		public void Start()
		{
			modifierContainer.Add(this);
		}

		public void Stop()
		{
			modifierContainer.Remove(this);
		}
	}
}