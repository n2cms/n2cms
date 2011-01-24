using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions;
using N2.Persistence.Finder;
using N2.Web;
using N2.Engine;
using N2.Edit.Trash;

namespace N2.Persistence
{
	[Service]
	public class DescendantItemFinder
	{
		#region Private Fields
		readonly IDefinitionManager definitions;
		readonly IItemFinder finder;
		#endregion

		#region Constructors
		public DescendantItemFinder(IItemFinder finder, IDefinitionManager definitions)
		{
			this.finder = finder;
			this.definitions = definitions;
		}
		#endregion

		public virtual IEnumerable<T> Find<T>(ContentItem root) where T:class
		{
			if (root is T)
				yield return root as T;

			foreach (ItemDefinition definition in definitions.GetDefinitions())
			{
				if (typeof(T).IsAssignableFrom(definition.ItemType))
				{
					foreach(var item in finder
						.Where.Type.Eq(definition.ItemType)
						.And.AncestralTrail.Like(Utility.GetTrail(root) + "%")
						.Select())
					{
						if (N2.Find.EnumerateParents(item).Any(i => i is ITrashCan))
							continue;

						yield return item as T;
					}
				}
			}
		}
	}
}
