using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Search
{
	/// <summary>
	/// Text that can be indexed.
	/// </summary>
	public class IndexableContent
	{
		public string Name { get; set; }
		public string TextContent { get; set; }
	}
}
