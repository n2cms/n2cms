using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Definitions.Runtime
{
	public class ViewTemplateDescription
	{
		public IEnumerable<string> TouchedPaths { get; set; }
		public ContentRegistration Registration { get; set; }
		public ItemDefinition Definition { get; set; }
	}
}