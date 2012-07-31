using System;
using System.Linq;
using System.Collections.Generic;
using N2.Details;
using N2.Collections;
using System.Linq.Expressions;

namespace N2.Definitions.Runtime
{
	/// <summary>
	/// Used to register editors and other information about content of the specified generic type.
	/// </summary>
	/// <typeparam name="TModel">The type of content to define.</typeparam>
	public class ContentRegistration<TModel> : ContentRegistration, IContentRegistration<TModel>
	{
		public ContentRegistration(ItemDefinition definition)
			: base(definition)
		{
			DefaultConventions = new RegistrationConventions();
		}

		public RegistrationConventions DefaultConventions { get; set; }

		/// <summary>Begins registration of a property on the content type.</summary>
		/// <typeparam name="TProperty">The property type of the property.</typeparam>
		/// <param name="detailName">An the name of the detail to define.</param>
		/// <returns>A property registration object.</returns>
		public PropertyRegistration<TModel, TProperty> On<TProperty>(string detailName)
		{
			return new PropertyRegistration<TModel, TProperty>(this, detailName);
		}

		ContainerBuilder<TModel, T> IContentRegistration<TModel>.Register<T>(T container)
		{
			Add(container);
			return new ContainerBuilder<TModel, T>(container.Name, this);
		}
	}

	/// <summary>
	/// Used to register editors and other information about a content type.
	/// </summary>
	public class ContentRegistration : IContentRegistration, IRegistration
	{
		public class ContentRegistrationContext
		{
			public ContentRegistrationContext()
			{
				TouchedPaths = new List<string>();
			}

			/// <summary>The touched paths are used for setting up cache invalidation of this registratin.</summary>
			public ICollection<string> TouchedPaths { get; private set; }

			/// <summary>Container name is used while contents of an editor container.</summary>
			public string ContainerName { get; set; }
			/// <summary>The current sorter is maintained o add editors in a sequence.</summary>
			public int CurrentSortOrder { get; set; }
			/// <summary>The global sort offset is used when registering sub-components.</summary>
			public int GlobalSortOffset { get; set; }

			/// <summary>Gets an incremental sort order for editables.</summary>
			/// <param name="proposedSortOrder">A sort order override parameter that overwrites the existing current sort order.</param>
			/// <returns>The previous sort order plus a default sort increment.</returns>
			public int NextSortOrder(int? proposedSortOrder)
			{
				CurrentSortOrder = proposedSortOrder ?? (CurrentSortOrder + DefaultSortIncrement);
				return CurrentSortOrder;
			}

			/// <summary>Sets the current container and resets it upon return value disposal.</summary>
			/// <param name="containerName">The container to use within the using statement.</param>
			/// <returns>Resets the container upon disposal.</returns>
			public IDisposable BeginContainer(string containerName)
			{
				var previous = ContainerName;
				ContainerName = containerName;

				return new Engine.Globalization.Scope(() => ContainerName = previous);
			}
		}

		public const int DefaultSortIncrement = 10;

		public ContentRegistration(ItemDefinition definition)
		{
			Refiners = new List<ISortableRefiner>();
			Definition = definition;
			ContentType = Definition.ItemType;
			Context = new ContentRegistrationContext();
		}

		public ContentRegistrationContext Context { get; set; }

		public Type ContentType { get; set; }

		public ItemDefinition Definition { get; set; }

		public ICollection<ISortableRefiner> Refiners { get; set; }
		
		/// <summary>Immediately maps to the definition title.</summary>
		public string Title 
		{
			get { return Definition.Title; }
			set { Definition.Title = value; }
		}
		
		/// <summary>This property is set to true when the view start registering.</summary>
		public bool IsDefined { get; set; }


		public ContentRegistration Add(ISortableRefiner refiner)
		{
			Refiners.Add(refiner);

			return this;
		}

		public ContentRegistration Add(IUniquelyNamed named)
		{
			Definition.Add(named);

			return this;
		}

		public ContentRegistration Add(IContainable containable)
		{
			Definition.Add(containable);
			containable.ContainerName = Context.ContainerName;

			return this;
		}

		public ContentRegistration Add(IEditable editable, string title)
		{
			editable.Title = title;
			editable.SortOrder = Context.NextSortOrder(null);
			Add(editable);

			return this;
		}

		public ContentRegistration Add(IEditable editable, string name, string title)
		{
			editable.Name = name;
			Add(editable, title ?? name);

			return this;
		}

		public ItemDefinition Finalize()
		{
			if (IsDefined)
			{
				Definition.IsDefined = true;

				foreach (var refiner in Refiners.OrderBy(r => r.RefinementOrder))
					refiner.Refine(Definition, new[] { Definition });
			}

			return Definition;
		}

		public void Configure<T>(string propertyName, Action<T> configurationExpression)
		{
			foreach (var a in Definition.GetCustomAttributes<T>(propertyName))
				configurationExpression(a);
		}

		#region IContentRegistration Members
		
		EditableBuilder<T> IContentRegistration.RegisterEditable<T>(string name, string title)
		{
			Add(new T(), name, title);
			return new EditableBuilder<T>(name, this);
		}
		EditableBuilder<T> IContentRegistration.RegisterEditable<T>(T editable)
		{
			Add(editable);
			return new EditableBuilder<T>(editable.Name, this);
		}

		#endregion

		#region IRegistration Members

		Builder<T> IRegistration.Register<T>(T container)
		{
			Add(container);
			return new Builder<T>(container.Name, this);
		}
		public void RegisterModifier(IContentTransformer modifier)
		{
			Definition.ContentTransformers.Add(modifier);
		}

		public Builder<T> RegisterRefiner<T>(T refiner) where T : ISortableRefiner
		{
			Add(refiner);
			return new InstanceBuilder<T>(this) { Instance = refiner };
		}

		#region class InstanceBuilder
		class InstanceBuilder<T> : Builder<T>
		{
			public InstanceBuilder(ContentRegistration re)
				: base(re)
			{
			}

			public T Instance { get; set; }

			public override Builder<T> Configure(Action<T> configurationExpression)
			{
				if (Registration != null && configurationExpression != null)
					configurationExpression(Instance);
				return this;
			}
		}
		#endregion
		#endregion
	}
}