using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions;

namespace N2.Edit.Templating
{
	public class TemplateInfo
	{
		public string Name { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }

		public ContentItem Template { get; set; }
		public ItemDefinition Definition { get; set; }

		//public IEnumerable<string> HiddenEditors { get; set; }
	}
}
