using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Definitions
{
	/// <summary>
	/// Provides the ability to create items with alternative editors, and pre-canned content.
	/// </summary>
	public interface ITemplateProvider
	{
		/// <summary>Gets all templates for the given content type</summary>
		/// <param name="contentType">The type of content item whose templates to get.</param>
		/// <returns>An emumeration of templates of the given type.</returns>
		IEnumerable<TemplateDefinition> GetTemplates(Type contentType);

		/// <summary>Gives the template definition for items created by this template.</summary>
		/// <param name="item">The item whose template definition to get.</param>
		/// <returns>The corresponding template definition or null if the item doesn't derive from this provider.</returns>
		TemplateDefinition GetTemplate(ContentItem item);
	}
}
