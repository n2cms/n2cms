﻿using System;
using System.Linq;
using System.Collections.Generic;
using N2.Details;
using N2.Collections;

namespace N2.Definitions.Runtime
{
	public class ContentRegistration : IContentRegistration, IRegistration
	{
		public ContentRegistration()
		{
			Containables = new ContentList<IUniquelyNamed>();
			TouchedPaths = new List<string>();
			ContentModifiers = new List<IContentTransformer>();
			Refiners = new List<ISortableRefiner>();
			DefaultSortIncrement = 10;
		}



		public Type ContentType { get; set; }
		public ContentList<IUniquelyNamed> Containables { get; private set; }
		public ICollection<IContentTransformer> ContentModifiers { get; set; }
		public ICollection<ISortableRefiner> Refiners { get; set; }
		public ICollection<string> TouchedPaths { get; private set; }
		public string ContainerName { get; set; }
		public int CurrentSortOrder { get; set; }
		public int GlobalSortOffset { get; set; }
		public int DefaultSortIncrement { get; set; }
		public bool Ignore { get; set; }
		public string Discriminator { get; set; }
		public string TemplateKey { get; set; }
		public string Title { get; set; }
		public bool IsDefined { get; set; }
		public bool ReplaceDefault { get; set; }



		public ContentRegistration Add(ISortableRefiner refiner)
		{
			Refiners.Add(refiner);

			return this;
		}

		public ContentRegistration Add(IUniquelyNamed named)
		{
			Containables.Add(named);

			return this;
		}

		public ContentRegistration Add(IContainable containable)
		{
			Containables.Add(containable);
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

		public ItemDefinition AppendToDefinition(ItemDefinition definition)
		{
			definition.Title = Title;
			definition.TemplateKey = TemplateKey;

			foreach (var c in Containables)
				definition.Add(c);

			foreach (var dv in ContentModifiers)
				definition.ContentTransformers.Add(dv);

			foreach (var refiner in Refiners.OrderBy(r => r.RefinementOrder))
				refiner.Refine(definition, new[] { definition });

			return definition;
		}

		public void Configure<T>(string propertyName, Action<T> configurationExpression)
		{
			configurationExpression((T)Containables[propertyName]);
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
			ContentModifiers.Add(modifier);
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