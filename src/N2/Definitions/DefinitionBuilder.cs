using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using N2.Details;
using N2.Engine;
using N2.Web.UI;
using N2.Configuration;

namespace N2.Definitions
{
	/// <summary>
	/// Inspects available types in the AppDomain and builds item definitions.
	/// </summary>
	public class DefinitionBuilder
	{
		private readonly ITypeFinder typeFinder;
		readonly EngineSection config;
		private readonly EditableHierarchyBuilder<IEditable> hierarchyBuilder;
		private readonly AttributeExplorer<EditorModifierAttribute> modifierExplorer;
		private readonly AttributeExplorer<IDisplayable> displayableExplorer;
		private readonly AttributeExplorer<IEditable> editableExplorer;
		private readonly AttributeExplorer<IEditableContainer> containableExplorer;
		
		public DefinitionBuilder(ITypeFinder typeFinder, EngineSection config)
			: this(typeFinder, config, new EditableHierarchyBuilder<IEditable>(), new AttributeExplorer<EditorModifierAttribute>(), new AttributeExplorer<IDisplayable>(), new AttributeExplorer<IEditable>(), new AttributeExplorer<IEditableContainer>())
		{
		}

		protected DefinitionBuilder(ITypeFinder typeFinder, EngineSection config, EditableHierarchyBuilder<IEditable> hierarchyBuilder, AttributeExplorer<EditorModifierAttribute> modifierExplorer, AttributeExplorer<IDisplayable> displayableExplorer, AttributeExplorer<IEditable> editableExplorer, AttributeExplorer<IEditableContainer> containableExplorer)
		{
			this.typeFinder = typeFinder;
			this.config = config;
			this.hierarchyBuilder = hierarchyBuilder;
			this.modifierExplorer = modifierExplorer;
			this.displayableExplorer = displayableExplorer;
			this.editableExplorer = editableExplorer;
			this.containableExplorer = containableExplorer;
		}

		/// <summary>Builds item definitions in the current environment.</summary>
		/// <returns>A dictionary of item definitions in the current environment.</returns>
		public virtual IDictionary<Type, ItemDefinition> GetDefinitions()
		{
			List<ItemDefinition> definitions = FindDefinitions();
			ExecuteRefiners(definitions);

			foreach (DefinitionElement element in config.Definitions.RemovedElements)
			{
				ItemDefinition definition = definitions.Find(d => d.Discriminator == element.Name);
				if (definition == null)
					throw new ConfigurationErrorsException("The configuration element /n2/engine/definitions/remove references a definition '" + element.Name + "' that couldn't be found in the application. Either remove the <remove/> configuration or ensure that the corresponding content class exists.");

				definition.IsDefined = false;
			} 
			
			return ToDictionary(definitions);
		}

		protected List<ItemDefinition> FindDefinitions()
		{
			List<ItemDefinition> definitions = new List<ItemDefinition>();
            foreach (Type itemType in FindConcreteTypes())
			{
				ItemDefinition definition = new ItemDefinition(itemType);
				ExploreAndLoad(definition);
				definitions.Add(definition);
			}

			foreach(DefinitionElement element in config.Definitions.AddedElements)
			{
				Type itemType = Type.GetType(element.Type);
				if (itemType == null)
					throw new ConfigurationErrorsException("The configuration element /n2/engine/definitions/add references a type '" + element.Type + "' that could not be loaded. Check the spelling and ensure that the type is available to the application.");
				if (!typeof(ContentItem).IsAssignableFrom(itemType))
					throw new ConfigurationErrorsException("The configuration element /n2/engine/definitions/add references a type '" + element.Type + "' that doesn't derive from N2.ContentItem. Ensure that the base class is N2.ContentItem.");

				ItemDefinition definition = definitions.Find(d => d.Discriminator == element.Name);
				if(definition == null)
				{
					definition = new ItemDefinition(itemType);
					ExploreAndLoad(definition);
				}

				definition.Discriminator = element.Name;
				definition.SortOrder = element.SortOrder;
				definition.Title = element.Title;
				definition.ToolTip = element.ToolTip;
				definition.IsDefined = true;
				definitions.Add(definition);
			}

			definitions.Sort();
			return definitions;
		}

		void ExploreAndLoad(ItemDefinition definition)
		{
			definition.Editables = editableExplorer.Find(definition.ItemType);
			definition.Containers = containableExplorer.Find(definition.ItemType);
			definition.Modifiers = modifierExplorer.Find(definition.ItemType);
			definition.Displayables = displayableExplorer.Find(definition.ItemType);
			definition.RootContainer = hierarchyBuilder.Build(definition.Containers, definition.Editables);
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
			// these are executed one time per definition
			List<ISortableRefiner> globalRefiners = new List<ISortableRefiner>();
			foreach (Assembly a in typeFinder.GetAssemblies())
				foreach (IDefinitionRefiner refiner in a.GetCustomAttributes(typeof(IDefinitionRefiner), false))
					globalRefiners.Add(refiner);

			// build the whole list of refiners
			List<RefinerPair> refiners = new List<RefinerPair>();
			foreach (ItemDefinition definition in definitions)
			{
				foreach (IDefinitionRefiner refiner in globalRefiners)
					refiners.Add(new RefinerPair(definition, refiner));
				foreach (IDefinitionRefiner refiner in definition.ItemType.GetCustomAttributes(typeof(IDefinitionRefiner), false))
					refiners.Add(new RefinerPair(definition, refiner));
				foreach (IInheritableDefinitionRefiner refiner in definition.ItemType.GetCustomAttributes(typeof(IInheritableDefinitionRefiner), true))
					refiners.Add(new RefinerPair(definition, refiner));
			}

			// sort them and execute
			refiners.Sort((first, second) => first.Refiner.CompareTo(second.Refiner));
			foreach (RefinerPair pair in refiners)
				pair.Refiner.Refine(pair.Definition, definitions);
		}

		protected class RefinerPair
		{
			public RefinerPair(ItemDefinition definition, ISortableRefiner refiner)
			{
				Definition = definition;
				Refiner = refiner;
			}
			public ItemDefinition Definition { get; set; }
			public ISortableRefiner Refiner { get; set; }
		}

        /// <summary>Enumerates concrete item types provided by the type finder.</summary>
        /// <returns>An enumeration of types derived from <see cref="N2.ContentItem"/>.</returns>
		protected IEnumerable<Type> FindConcreteTypes()
		{
			foreach(Type t in typeFinder.Find(typeof (ContentItem)))
			{
				if(t != null && !t.IsAbstract && !t.ContainsGenericParameters)
				{
                    yield return t;
				}
			}
		}
	}
}
