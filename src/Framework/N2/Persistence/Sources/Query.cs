using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Collections;
using N2.Edit;

namespace N2.Persistence.Sources
{
	public class Query
	{
		public Query()
		{
			Interface = Interfaces.Viewing;
		}

		public ContentItem Parent { get; set; }
		public string Interface { get; set; }
		public bool? OnlyPages { get; set; }
		public bool SkipAuthorization { get; set; }

		public ItemFilter Filter { get; set; }

		public ParameterCollection AsParameters()
		{
			var p = Parameter.Equal("Parent", Parent);
			if (OnlyPages.HasValue)
				return p & (OnlyPages.Value ? Parameter.IsNull("ZoneName") : Parameter.IsNotNull("ZoneName"));

			return p;
		}
	}
}
