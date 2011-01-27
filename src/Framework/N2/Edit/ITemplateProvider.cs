using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions;

namespace N2.Edit
{
	public interface ITemplateProvider
	{
		IEnumerable<TemplateDefinition> GetTemplates(Type contentType);
	}

	public static class TemplateProviderExtensions
	{
		public static TemplateDefinition GetTemplate(this IEnumerable<ITemplateProvider> providers, Type contentType, string templateName)
		{
			return providers.SelectMany(tp => tp.GetTemplates(contentType)).FirstOrDefault(td => td.Name == templateName);
		}
	}
}
