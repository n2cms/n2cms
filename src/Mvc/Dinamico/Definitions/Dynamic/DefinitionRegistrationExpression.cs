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
			Containables = new Dictionary<string, IUniquelyNamed>();
			DefaultSortIncrement = 10;
		}



		public IDictionary<string, IUniquelyNamed> Containables { get; private set; }
		public string ContainerName { get; set; }
		public int CurrentSortOrder { get; set; }
		public int GlobalSortOffset { get; set; }
		public int DefaultSortIncrement { get; set; }
		public Type ItemType { get; set; }
		public bool Ignore { get; set; }
		public string Discriminator { get; set; }
		public string Template { get; set; }
		public string Title { get; set; }
		public bool IsDefined { get; set; }



		public DefinitionRegistrationExpression Add(IContainable containable)
		{
			Containables[containable.Name] = containable;
			containable.ContainerName = ContainerName;

			return this;
		}

		public DefinitionRegistrationExpression Add(IEditable editable, string title)
		{
			editable.Title = title;
			editable.SortOrder = NextSortOrder(null);
			Add(editable);

			return this;
		}

		public DefinitionRegistrationExpression Add(IEditable editable, string name, string title)
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

		public ItemDefinition CreateDefinition(Definitions.Static.DefinitionTable definitions)
		{
			var id = definitions.GetDefinition(ItemType).Initialize(ItemType).Clone();

			foreach (IDefinitionRefiner refiner in ItemType.GetCustomAttributes(typeof(IDefinitionRefiner), true))
				refiner.Refine(id, new[] { id });

			id.Title = Title;
			id.Template = Template;

			foreach (var c in Containables)
				id.Add(c.Value);

			return id;
		}

		public void Configure<T>(string propertyName, Action<T> configurationExpression)
		{
			configurationExpression((T)Containables[propertyName]);
		}
	}
}