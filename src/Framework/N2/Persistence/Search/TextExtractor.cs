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

		public virtual bool IsIndexable(ContentItem item)
		{
			if (item.GetContentType().GetCustomAttributes(true).OfType<IIndexableType>().Any(it => !it.IsIndexable))
				return false;

			return true;
		}

		public virtual IEnumerable<IndexableContent> Extract(ContentItem item)
		{
			return extractors.SelectMany(e => e.Extract(item)).Where(ic => !string.IsNullOrEmpty(ic.TextContent));
		}

		public virtual string Join(IEnumerable<IndexableContent> content)
		{
			StringBuilder sb = new StringBuilder();
			foreach (var c in content)
				sb.AppendLine(c.TextContent);
			return sb.ToString();
		}
	}
}
