using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions;

namespace N2.Edit
{
	/// <summary>
	/// Describes a template for content
	/// </summary>
	public class ContentTemplate
	{
		/// <summary>The name is used to refernce this template.</summary>
		public string Name { get; set; }

		/// <summary>Short description of the template.</summary>
		public string Title { get; set; }
		/// <summary>Longer description of the template.</summary>
		public string Description { get; set; }

		/// <summary>The url where the template can be observed.</summary>
		public string TemplateUrl { get; set; }

		/// <summary>A copy of the template.</summary>
		public ContentItem Template { get; set; }

		/// <summary>The original template.</summary>
		public ContentItem Original { get; set; }

		/// <summary>The item definition of the template.</summary>
		public ItemDefinition Definition { get; set; }

		//public IEnumerable<string> HiddenEditors { get; set; }
	}
}
