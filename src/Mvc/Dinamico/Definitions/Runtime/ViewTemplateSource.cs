using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Definitions.Runtime
{
	public class ViewTemplateSource
	{
		public string ControllerName { get; set; }
		public Type ModelType { get; set; }
		public string TemplateSelectorContainerName { get; set; }
		public string ViewFileExtension { get; set; }
	}
}