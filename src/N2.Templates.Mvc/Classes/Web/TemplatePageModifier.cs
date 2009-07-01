using System.Collections.Generic;
using N2.Engine.Globalization;
using N2.Web;

namespace N2.Templates.Mvc.Web
{
	/// <summary>
	/// Performs operations on the page template on each request.
	/// </summary>
	public class TemplatePageModifier : IPageModifierContainer
	{
		private readonly IList<IPageModifier> modifiers = new List<IPageModifier>();

		public TemplatePageModifier()
		{
			modifiers.Add(new ThemeModifier());
			modifiers.Add(new MasterPageModifier());
		}

		public TemplatePageModifier(ILanguageGateway gateway, IUrlParser urlParser)
			: this()
		{
			if (gateway.Enabled)
				modifiers.Add(new LanguageModifier(gateway, urlParser));
		}

		public void Add(IPageModifier modifier)
		{
			modifiers.Add(modifier);
		}

		public void Remove(IPageModifier modifier)
		{
			modifiers.Remove(modifier);
		}

		public void Modify(Page page)
		{
			foreach (IPageModifier adapter in modifiers)
			{
				adapter.Modify(page);
			}
		}
	}
}