using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;

namespace N2.Persistence.Search
{
	/// <summary>
	/// Extracts text from content.
	/// </summary>
	[Service]
	public class TextExtractor
	{
		ITextExtractor[] extractors;

		public TextExtractor(params ITextExtractor[] extractors)
		{
			this.extractors = extractors;
		}

		public virtual IEnumerable<IndexableContent> Extract(ContentItem item)
		{
			return extractors.SelectMany(e => e.Extract(item));
		}
	}
}
