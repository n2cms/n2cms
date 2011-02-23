using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions;

namespace N2.Definitions
{
	public class AllowedDefinitionQuery
	{
		public IDefinitionManager Definitions { get; set; }
		public ContentItem Parent { get; set; }
		public ItemDefinition ParentDefinition { get; set; }
		public ItemDefinition ChildDefinition { get; set; }
	}
}
