using System.Web.UI;
using Castle.Core;
using N2.Templates.Mvc.Web;
using N2.Plugin;
using N2.Web;

namespace N2.Templates.Mvc.Services
{
	public class SEOPageModifier : IPageModifier, IAutoStart
	{
		private readonly IPageModifierContainer modifierContainer;
		private readonly IUrlParser urlParser;

		public SEOPageModifier(IPageModifierContainer modifierContainer, IUrlParser urlParser)
		{
			this.modifierContainer = modifierContainer;
			this.urlParser = urlParser;
		}

		public void Modify(Page page)
		{
			new TitleAndMetaTagApplyer(page, urlParser.CurrentPage);
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