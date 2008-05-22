using Castle.Core;
using N2.Templates.Items;
using N2.Templates.Web;
using N2.Templates.Web.UI;

namespace N2.Templates.SEO
{
	public class SEOPageModifier : IPageModifier, IStartable
	{
		private readonly IPageModifierContainer modifierContainer;

		public SEOPageModifier(IPageModifierContainer modifierContainer)
		{
			this.modifierContainer = modifierContainer;
		}

		public void Modify<T>(TemplatePage<T> page) where T : AbstractPage
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
