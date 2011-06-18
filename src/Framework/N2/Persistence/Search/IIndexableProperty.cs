using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Search
{
	public interface IIndexableProperty
	{
		bool Index { get; }

		string Name { get; }

		string GetIndexableText(ContentItem item);
	}
}
