using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Details;

namespace N2.Definitions.Dynamic
{
	public class DefinitionRegistrationExpression
	{
		public DefinitionRegistrationExpression()
		{
			Containables = new List<IUniquelyNamed>();
		}



		public IList<IUniquelyNamed> Containables { get; private set; }
		public string ContainerName { get; set; }
		public int CurrentSortOrder { get; set; }
		public int SortOffset { get; set; }
		public Type ItemType { get; set; }
		public bool Ignore { get; set; }
		public string Discriminator { get; set; }
		public string Template { get; set; }
		public string Title { get; set; }
		public bool IsDefined { get; set; }



		public DefinitionRegistrationExpression AddContainable<T>(T containable) where T : IContainable
		{
			Containables.Add(containable);
			containable.ContainerName = ContainerName;

			return this;
		}

		public DefinitionRegistrationExpression AddContainable<T>(T containable, Action<T> config) where T : IContainable
		{
			AddContainable(containable);

			if (config != null) config(containable);

			return this;
		}

		public DefinitionRegistrationExpression AddDisplayable<T>(T containable) where T : IDisplayable
		{
			Containables.Add(containable);

			return this;
		}

		public DefinitionRegistrationExpression AddDisplayable<T>(T containable, Action<T> config) where T : IDisplayable
		{
			AddDisplayable(containable);

			if (config != null) config(containable);

			return this;
		}

		public DefinitionRegistrationExpression AddEditable<T>(T editable, string title, Action<T> config) where T : IEditable
		{
			AddContainable(editable, null);
			editable.Title = title;
			editable.SortOrder = NextSortOrder(null);
			if (config != null) config(editable);

			return this;
		}

		public DefinitionRegistrationExpression AddEditable<T>(T editable, string name, string title, Action<T> config) where T : IEditable
		{
			AddEditable(editable, title ?? name, null);
			editable.Name = name;
			if (config != null) config(editable);

			return this;
		}

		public int NextSortOrder(int? proposedSortOrder)
		{
			CurrentSortOrder = proposedSortOrder ?? ++CurrentSortOrder;
			return CurrentSortOrder + SortOffset;
		}

		public ItemDefinition CreateDefinition(Definitions.Static.DefinitionTable definitions)
		{
			var id = definitions.GetDefinition(ItemType).Initialize(ItemType).Clone();

			foreach (IDefinitionRefiner refiner in ItemType.GetCustomAttributes(typeof(IDefinitionRefiner), true))
				refiner.Refine(id, new[] { id });

			id.Title = Title;
			id.Template = Template;

			foreach (var c in Containables)
				id.Add(c);

			return id;
		}
	}
}