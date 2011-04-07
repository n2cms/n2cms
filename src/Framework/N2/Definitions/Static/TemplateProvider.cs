using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;

namespace N2.Definitions.Static
{
	[Service(typeof(ITemplateProvider))]
	public class TemplateProvider : ITemplateProvider
	{
		DefinitionMap map;

		public TemplateProvider(DefinitionMap map)
		{
			this.map = map;
		}

		#region ITemplateProvider Members

		public IEnumerable<TemplateDefinition> GetTemplates(Type contentType)
		{
			yield return CreateTemplate(map.GetOrCreateDefinition(contentType));
		}

		public TemplateDefinition GetTemplate(ContentItem item)
		{
			if (item["TemplatName"] != null)
				return null;

			var template = CreateTemplate(map.GetOrCreateDefinition(item));
			template.Original = item;
			template.Template = item.Clone(false);
			return template;
		}

		private TemplateDefinition CreateTemplate(ItemDefinition itemDefinition)
		{
			return new TemplateDefinition
			{
				Definition = itemDefinition,
				Description = itemDefinition.Description,
				Name = null,
				Title = itemDefinition.Title
			};
		}

		#endregion
	}
}
