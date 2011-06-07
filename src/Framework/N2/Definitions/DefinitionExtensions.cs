using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Definitions
{
	public static class DefinitionExtensions
	{
		public static IEnumerable<ItemDefinition> AllowedBelow(this IEnumerable<ItemDefinition> allDefinitions, ContentItem parentItem, IDefinitionManager definitions)
		{
			var parentDefinition = definitions.GetDefinition(parentItem);
			foreach (var definition in allDefinitions)
			{
				if (IsAllowed(definition, parentItem, parentDefinition, definitions))
					yield return definition;
			}
		}

		public static IEnumerable<TemplateDefinition> AllowedBelow(this IEnumerable<TemplateDefinition> allTemplates, ContentItem parentItem, IDefinitionManager definitions)
		{
			var parentDefinition = definitions.GetDefinition(parentItem);
			foreach (var template in allTemplates)
			{
				if (IsAllowed(template.Definition, parentItem, parentDefinition, definitions))
					yield return template;
			}
		}

		private static bool IsAllowed(ItemDefinition definition, ContentItem parentItem, ItemDefinition parentDefinition, IDefinitionManager definitions)
		{
			var ctx = new AllowedDefinitionQuery { Parent = parentItem, ParentDefinition = parentDefinition, ChildDefinition = definition, Definitions = definitions };
			var filters = parentDefinition.AllowedChildFilters.Union(definition.AllowedParentFilters).ToList();
			if (filters.Any(f => f.IsAllowed(ctx) == AllowedDefinitionResult.Allow))
				return true;
			else if (!filters.Any(f => f.IsAllowed(ctx) == AllowedDefinitionResult.Deny))
				return true;
			return false;
		}
	}
}
