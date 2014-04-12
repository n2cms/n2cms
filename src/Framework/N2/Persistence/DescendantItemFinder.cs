using System.Collections.Generic;
using System.Linq;
using N2.Definitions;
using N2.Edit.Trash;
using N2.Engine;
using N2.Persistence.Finder;

namespace N2.Persistence
{
    [Service]
    public class DescendantItemFinder
    {
        #region Private Fields
        readonly IDefinitionManager definitions;
        readonly IContentItemRepository repository;
        #endregion

        #region Constructors
        public DescendantItemFinder(IContentItemRepository repository, IDefinitionManager definitions)
        {
            this.repository = repository;
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
                    foreach (var item in repository.FindDescendants(root, definition.Discriminator).ToList())
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
