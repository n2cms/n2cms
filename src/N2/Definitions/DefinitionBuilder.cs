using System;
using System.Collections.Generic;
using System.Reflection;
using N2.Details;
using N2.Engine;
using N2.Web.UI;

namespace N2.Definitions
{
	/// <summary>
	/// Inspects available types in the AppDomain and builds item definitions.
	/// </summary>
	public class DefinitionBuilder
	{
		private readonly ITypeFinder typeFinder;
		private readonly EditableHierarchyBuilder<IEditable> hierarchyBuilder;
		private readonly AttributeExplorer<EditorModifierAttribute> modifierExplorer;
		private readonly AttributeExplorer<IDisplayable> displayableExplorer;
		private readonly AttributeExplorer<IEditable> editableExplorer;
		private readonly AttributeExplorer<IEditableContainer> containableExplorer;
		
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
			List<ItemDefinition> definitions = new List<ItemDefinition>();
            foreach (Type itemType in FindConcreteTypes())
			{
				ItemDefinition definition = new ItemDefinition(itemType);
				definition.Editables = editableExplorer.Find(definition.ItemType);
				definition.Containers = containableExplorer.Find(definition.ItemType);
				definition.Modifiers = modifierExplorer.Find(definition.ItemType);
				definition.Displayables = displayableExplorer.Find(definition.ItemType);
				definition.RootContainer = hierarchyBuilder.Build(definition.Containers, definition.Editables);
				definitions.Add(definition);
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
				if(t != null && !t.IsAbstract)
				{
                    yield return t;
				}
			}
		}
	}
}