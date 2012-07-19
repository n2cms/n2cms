using System;
using System.Collections.Generic;
using System.Linq;
using N2.Engine;
using N2.Persistence;

namespace N2.Definitions.Static
{
	[Service(typeof(ITemplateProvider))]
	public class TemplateProvider : ITemplateProvider
	{
		ContentActivator activator;
		DefinitionMap map;

		public TemplateProvider(ContentActivator activator, DefinitionMap map)
		{
			this.activator = activator;
			this.map = map;
			SortOrder = 1000;
		}

		#region ITemplateProvider Members

		public IEnumerable<TemplateDefinition> GetTemplates(Type contentType)
		{
			yield return CreateTemplate(map.GetOrCreateDefinition(contentType));

			foreach (var ta in N2.Web.PathDictionary.GetFinders(contentType).OfType<N2.Web.TemplateAttribute>().Where(ta => ta.SelectableAsDefault))
			{
				string templateKey = ta.Action;
				var definition = map.GetOrCreateDefinition(contentType, templateKey);
				var template = CreateTemplate(definition);
				template.OriginalFactory = () => null;
				template.TemplateFactory = () => activator.CreateInstance(contentType, null, templateKey);
				template.Title = ta.TemplateTitle ?? definition.Title;
				template.Description = ta.TemplateDescription ?? definition.Description;
				template.Name = templateKey;
				yield return template;
			}
		}

		public TemplateDefinition GetTemplate(ContentItem item)
		{
			var template = CreateTemplate(map.GetOrCreateDefinition(item));
			template.OriginalFactory = () => item;
			template.TemplateFactory = () => item.Clone(false);
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

		/// <summary>The order this template provider should be invoked, default 0.</summary>
		public int SortOrder { get; set; }
	}
}
