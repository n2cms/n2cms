using System;
using System.Collections.Generic;
using System.Reflection;
using N2.Details;
using N2.Engine;
using N2.Web.UI;

namespace N2.Definitions
{
	/// <summary>
	/// This class is responsible for inspecting available types and providing 
	/// item definitions to the definition manager.
	/// </summary>
	public class DefinitionBuilder
	{
		private readonly ITypeFinder typeFinder;
		private readonly EditableHierarchyBuilder<IEditable> hierarchyBuilder;
		private readonly AttributeExplorer<EditorModifierAttribute> modifierExplorer;
		private readonly AttributeExplorer<IDisplayable> displayableExplorer;
		private readonly AttributeExplorer<IEditable> editableExplorer;
		private readonly AttributeExplorer<IEditableContainer> containableExplorer;
		private bool useBackwardsCompatibleDiscriminator = false;

		public DefinitionBuilder(ITypeFinder typeFinder)
			: this(typeFinder, new EditableHierarchyBuilder<IEditable>(), new AttributeExplorer<EditorModifierAttribute>(), new AttributeExplorer<IDisplayable>(), new AttributeExplorer<IEditable>(), new AttributeExplorer<IEditableContainer>())
		{
		}

		public DefinitionBuilder(ITypeFinder typeFinder, EditableHierarchyBuilder<IEditable> hierarchyBuilder, AttributeExplorer<EditorModifierAttribute> modifierExplorer, AttributeExplorer<IDisplayable> displayableExplorer, AttributeExplorer<IEditable> editableExplorer, AttributeExplorer<IEditableContainer> containableExplorer)
		{
			this.typeFinder = typeFinder;
			this.hierarchyBuilder = hierarchyBuilder;
			this.modifierExplorer = modifierExplorer;
			this.displayableExplorer = displayableExplorer;
			this.editableExplorer = editableExplorer;
			this.containableExplorer = containableExplorer;
		}

		/// <summary>Gets or sets wether the definitions use full name space as discriminator for backward compatibility.</summary>
		public bool UseBackwardsCompatibleDiscriminator
		{
			get { return useBackwardsCompatibleDiscriminator; }
			set { useBackwardsCompatibleDiscriminator = value; }
		}

		/// <summary>Builds item definitions in the current environment.</summary>
		/// <returns>A dictionary of item definitions in the current environment.</returns>
		public virtual IDictionary<Type, ItemDefinition> GetDefinitions()
		{
			IList<ItemDefinition> definitions = FindDefinitions();
			ExecuteRefiners(definitions);
			return ToDictionary(definitions);
		}

		protected List<ItemDefinition> FindDefinitions()
		{
			IList<Type> itemTypes = FindTypes();
			List<ItemDefinition> definitions = new List<ItemDefinition>();
			foreach (Type itemType in itemTypes)
			{
				if (!itemType.IsAbstract)
				{
					ItemDefinition definition = new ItemDefinition(itemType, UseBackwardsCompatibleDiscriminator);
					definition.Editables = editableExplorer.Find(definition.ItemType);
					definition.Containers = containableExplorer.Find(definition.ItemType);
					definition.Modifiers = modifierExplorer.Find(definition.ItemType);
					definition.Displayables = displayableExplorer.Find(definition.ItemType);
					definition.RootContainer = hierarchyBuilder.Build(definition.Containers, definition.Editables);
					definitions.Add(definition);
				}
			}
			definitions.Sort();
			return definitions;
		}

		protected static IDictionary<Type, ItemDefinition> ToDictionary(IList<ItemDefinition> definitions)
		{
			IDictionary<Type, ItemDefinition> definitionMap = new Dictionary<Type, ItemDefinition>();
			foreach(ItemDefinition definition in definitions)
			{
				definitionMap[definition.ItemType] = definition;
			}
			return definitionMap;
		}

		protected void ExecuteRefiners(IList<ItemDefinition> definitions)
		{
			foreach (ItemDefinition definition in definitions)
			{
				foreach (IDefinitionRefiner refiner in definition.ItemType.GetCustomAttributes(typeof(IDefinitionRefiner), false))
				{
					refiner.Refine(definition, definitions);
				}
			}
			foreach (ItemDefinition definition in definitions)
			{
				foreach (IInheritableDefinitionRefiner refiner in definition.ItemType.GetCustomAttributes(typeof(IInheritableDefinitionRefiner), true))
				{
					refiner.Refine(definition, definitions);
				}
			}
			foreach (ItemDefinition definition in definitions)
			{
				foreach (Assembly a in typeFinder.GetAssemblies())
				{
					foreach (IDefinitionRefiner refiner in a.GetCustomAttributes(typeof (IDefinitionRefiner), false))
					{
						refiner.Refine(definition, definitions);
					}
				}
			}
		}

		protected IList<Type> FindTypes()
		{
			List<Type> itemTypes = new List<Type>();
			foreach(Type t in typeFinder.Find(typeof (ContentItem)))
			{
				if(!t.IsAbstract)
				{
					itemTypes.Add(t);
				}
			}
			return itemTypes;
		}

		///// <summary>Gets the <see cref="ItemAttribute"/> of a certain type.</summary>
		///// <returns>The existing attribute or a new attribute.</returns>
		//protected virtual ItemAttribute GetItemAttribute(Type itemType)
		//{
		//    foreach (ItemAttribute a in itemType.GetCustomAttributes(typeof (ItemAttribute), false))
		//        return a;

		//    return new ItemAttribute(itemType.Name, itemType.FullName, string.Empty, itemType.FullName, 1000);
		//}

		//protected virtual IList<AvailableZoneAttribute> GetAvailableZones(Type itemType)
		//{
		//    List<AvailableZoneAttribute> availableZones = new List<AvailableZoneAttribute>();
		//    foreach (AvailableZoneAttribute a in itemType.GetCustomAttributes(typeof (AvailableZoneAttribute), true))
		//        availableZones.Add(a);
		//    return availableZones;
		//}

		//protected virtual IList<string> GetAllowedZones(Type itemType)
		//{
		//    List<string> allowedZoneNames = new List<string>();
		//    foreach (AllowedZonesAttribute a in itemType.GetCustomAttributes(typeof (AllowedZonesAttribute), true))
		//    {
		//        foreach (string zoneName in a.ZoneNames)
		//        {
		//            allowedZoneNames.Add(zoneName);
		//        }
		//    }
		//    return allowedZoneNames;
		//}

		//protected virtual IList<string> GetAuthorizedRoles(Type itemType)
		//{
		//    List<string> authorizedRoles = null;
		//    foreach (ItemAuthorizedRolesAttribute a in itemType.GetCustomAttributes(typeof (ItemAuthorizedRolesAttribute), true))
		//    {
		//        if (authorizedRoles == null)
		//            authorizedRoles = new List<string>();
		//        foreach (string role in a.Roles)
		//            if (!authorizedRoles.Contains(role))
		//                authorizedRoles.Add(role);
		//    }

		//    return authorizedRoles;
		//}
	}
}