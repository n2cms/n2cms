using System;
using System.Linq;
using System.Collections.Generic;
using N2.Details;
using N2.Collections;
using System.Linq.Expressions;

namespace N2.Definitions.Runtime
{
	public class PropertyRegistration<TModel, TProperty> : IPropertyRegistration<TProperty>
	{
		private ContentRegistration<TModel> registration;

		public string PropertyName { get; set; }
		public IContentRegistration Registration { get { return registration; } }

		public PropertyRegistration(ContentRegistration<TModel> registration, string expressionText)
		{
			this.registration = registration;
			this.PropertyName = expressionText;
		}


		public PropertyRegistration<TModel, TProperty> Add(IUniquelyNamed named)
		{
			named.Name = PropertyName;
			registration.Add(named);

			return this;
		}

		public PropertyRegistration<TModel, TProperty> Add(IContainable named)
		{
			named.Name = PropertyName;
			registration.Add(named);

			return this;
		}

		public PropertyRegistration<TModel, TProperty> Add(IEditable editable, string title)
		{
			editable.Name = PropertyName;
			registration.Add(editable, title);

			return this;
		}
	}

	public class ContentRegistration<TModel> : ContentRegistration, IContentRegistration<TModel>
	{
		public ContentRegistration(ItemDefinition definition)
			: base(definition)
		{
		}

		public PropertyRegistration<TModel, TProperty> On<TProperty>(Expression<Func<TModel, TProperty>> expression)
		{
			string expressionText = System.Web.Mvc.ExpressionHelper.GetExpressionText(expression);
			return new PropertyRegistration<TModel, TProperty>(this, expressionText);
		}

		public PropertyRegistration<TModel, TProperty> On<TProperty>(string detailName)
		{
			return new PropertyRegistration<TModel, TProperty>(this, detailName);
		}
	}

	public class ContentRegistration : IContentRegistration, IRegistration
	{
		public const int DefaultSortIncrement = 10;

		public ContentRegistration(ItemDefinition definition)
		{
			TouchedPaths = new List<string>();
			Refiners = new List<ISortableRefiner>();
			Definition = definition;
			ContentType = Definition.ItemType;
		}



		public Type ContentType { get; set; }

		public ItemDefinition Definition { get; set; }

		public ICollection<ISortableRefiner> Refiners { get; set; }
		
		/// <summary>The touched paths are used for setting up cache invalidation of this registratin.</summary>
		public ICollection<string> TouchedPaths { get; private set; }
		
		/// <summary>Container name is used while contents of an editor container.</summary>
		public string ContainerName { get; set; }
		/// <summary>The current sorter is maintained o add editors in a sequence.</summary>
		public int CurrentSortOrder { get; set; }
		/// <summary>The global sort offset is used when registering sub-components.</summary>
		public int GlobalSortOffset { get; set; }

		/// <summary>Immediately maps to the definition title.</summary>
		public string Title 
		{
			get { return Definition.Title; }
			set { Definition.Title = value; }
		}
		
		/// <summary>This property is set to true when the view start registering.</summary>
		public bool IsDefined { get; set; }

		//public IContentList<IUniquelyNamed> Containables 
		//{
		//    get { return new ContentList<IUniquelyNamed>(Definition.NamedOperators); }
		//}


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
			containable.ContainerName = ContainerName;

			return this;
		}

		public ContentRegistration Add(IEditable editable, string title)
		{
			editable.Title = title;
			editable.SortOrder = NextSortOrder(null);
			Add(editable);

			return this;
		}

		public ContentRegistration Add(IEditable editable, string name, string title)
		{
			editable.Name = name;
			Add(editable, title ?? name);

			return this;
		}

		public int NextSortOrder(int? proposedSortOrder)
		{
			CurrentSortOrder = proposedSortOrder ?? (CurrentSortOrder + DefaultSortIncrement);
			return CurrentSortOrder;
		}

		public ItemDefinition Finalize()
		{
			if (IsDefined)
				Definition.IsDefined = true;

			foreach (var refiner in Refiners.OrderBy(r => r.RefinementOrder))
				refiner.Refine(Definition, new[] { Definition });

			return Definition;
		}

		public void Configure<T>(string propertyName, Action<T> configurationExpression)
		{
			configurationExpression(Definition.NamedOperators.Where(no => no.Name == propertyName).OfType<T>().First());
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

		#region IContainerRegistration Members

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