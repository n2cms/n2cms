using System.Collections.Generic;

namespace N2.Definitions.Runtime
{
	public class ViewTemplateDescription
	{
		public IEnumerable<string> TouchedPaths { get; set; }
		public ContentRegistration Registration { get; set; }
		public ItemDefinition Definition { get; set; }
	}
}