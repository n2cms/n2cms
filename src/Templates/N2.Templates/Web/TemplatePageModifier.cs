using System.Collections.Generic;
using N2.Templates.Items;
using N2.Templates.Web.UI;
using N2.Globalization;

namespace N2.Templates.Web
{
	public class TemplatePageModifier : IPageModifierContainer
	{
		private readonly IList<IPageModifier> modifiers;

		public TemplatePageModifier()
		{
			this.modifiers = new List<IPageModifier>();
			this.modifiers.Add(new ThemeModifier());
			this.modifiers.Add(new MasterPageModifier());
		}

		public TemplatePageModifier(ILanguageGateway gateway)
			: this()
		{
			this.modifiers.Add(new LanguageModifier(gateway));
		}

		public void Add(IPageModifier modifier)
		{
			modifiers.Add(modifier);
		}

		public void Remove(IPageModifier modifier)
		{
			modifiers.Remove(modifier);
		}

		public void Modify<T>(TemplatePage<T> page)
			where T : AbstractPage
		{
			foreach (IPageModifier adapter in modifiers)
			{
				adapter.Modify(page);
			}
		}
	}
}
