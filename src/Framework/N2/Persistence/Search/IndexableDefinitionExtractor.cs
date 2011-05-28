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

		public IEnumerable<IndexableContent> Extract(ContentItem item)
		{
			foreach (var e in definitions.GetDefinition(item).Editables)
			{
				var indexable = e as IIndexableProperty;
				if (indexable == null || !indexable.Index)
					continue;

				yield return new IndexableContent { Name = e.Name, TextContent = indexable.GetIndexableText(item, e.Name) };
			}
		}

		#endregion
	}
}
