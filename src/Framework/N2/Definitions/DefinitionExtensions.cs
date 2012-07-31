using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Security;
using System.Security.Principal;

namespace N2.Definitions
{
	public static class DefinitionExtensions
	{
		public static IEnumerable<T> WhereAuthorized<T>(this IEnumerable<T> allSecurable, ISecurityManager security, IPrincipal user, ContentItem parentItem)
			where T : ISecurableBase
		{
			return allSecurable.Where(s => security.IsAuthorized(s, user, parentItem));
		}

		public static IEnumerable<ItemDefinition> AllowedBelow(this IEnumerable<ItemDefinition> allDefinitions, ItemDefinition parentDefinition, ContentItem parentItem, IDefinitionManager definitions)
		{
			foreach (var definition in allDefinitions)
			{
				if (IsAllowed(definition, parentItem, parentDefinition, definitions))
					yield return definition;
			}
		}

		public static IEnumerable<TemplateDefinition> AllowedBelow(this IEnumerable<TemplateDefinition> allTemplates, ItemDefinition parentDefinition, ContentItem parentItem, IDefinitionManager definitions)
		{
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
				// filter specificly allows -> allow
				return true;
			else if (!filters.Any(f => f.IsAllowed(ctx) == AllowedDefinitionResult.Deny))
				// no filter denies -> allow
				return true;

			// no filter allowed, but some filter denied -> deny
			return false;
		}

		public static PropertyDefinition GetOrCreate(this IDictionary<string, PropertyDefinition> properties, string name, Type propertyType)
		{
			PropertyDefinition property;
			if (!properties.TryGetValue(name, out property))
				properties[name] = property = new PropertyDefinition(name, propertyType);
			return property;
		}
	}
}
