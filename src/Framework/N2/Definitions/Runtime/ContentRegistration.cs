using System;
using System.Collections.Generic;
using N2.Details;
using N2.Web.Mvc;

namespace N2.Definitions.Runtime
{
	public class ContentRegistration : IContentRegistration, IRegistration
	{
		public ContentRegistration()
		{
			Containables = new Dictionary<string, IUniquelyNamed>();
			TouchedPaths = new List<string>();
			ContentModifiers = new List<IContentTransformer>();
			DefaultSortIncrement = 10;
		}



		public Type ContentType { get; set; }
		public IDictionary<string, IUniquelyNamed> Containables { get; private set; }
		public ICollection<IContentTransformer> ContentModifiers { get; set; }
		public ICollection<string> TouchedPaths { get; private set; }
		public string ContainerName { get; set; }
		public int CurrentSortOrder { get; set; }
		public int GlobalSortOffset { get; set; }
		public int DefaultSortIncrement { get; set; }
		public bool Ignore { get; set; }
		public string Discriminator { get; set; }
		public string Template { get; set; }
		public string Title { get; set; }
		public bool IsDefined { get; set; }
		public bool ReplaceDefault { get; set; }



		public ContentRegistration Add(IContainable containable)
		{
			Containables[containable.Name] = containable;
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

		public ItemDefinition AppendDefinition(ItemDefinition definition)
		{
			definition.Title = Title;
			definition.Template = Template;

			foreach (var c in Containables)
				definition.Add(c.Value);

			foreach (var dv in ContentModifiers)
				definition.ContentTransformers.Add(dv);

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

		#endregion

	}
}