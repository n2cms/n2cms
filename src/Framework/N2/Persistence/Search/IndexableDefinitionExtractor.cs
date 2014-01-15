using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions;
using N2.Engine;

namespace N2.Persistence.Search
{
    /// <summary>
    /// Extracts content from a <see cref="ContentItem"/> by looking for indexable properties via the item's definition.
    /// </summary>
    [Service(typeof(ITextExtractor))]
    public class IndexableDefinitionExtractor : ITextExtractor
    {
        IDefinitionManager definitions;

        public IndexableDefinitionExtractor(IDefinitionManager definitions)
        {
            this.definitions = definitions;
        }

        #region ITextExtractor Members

        public IEnumerable<IndexableField> Extract(ContentItem item)
        {
            foreach (var indexable in definitions.GetDefinition(item).NamedOperators.OfType<IIndexableProperty>())
            {
                if (indexable.IsIndexable)
                    yield return new IndexableField { Name = indexable.Name, Value = indexable.GetIndexableText(item), Analyzed = true, Stored = false };
            }
        }

        #endregion
    }
}
