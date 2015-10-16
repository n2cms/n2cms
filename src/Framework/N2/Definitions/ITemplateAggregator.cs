using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Definitions
{
    public interface ITemplateAggregator
    {
        /// <summary>Gets all templates for a certain content type.</summary>
        /// <param name="contentType">The type of a content item.</param>
        /// <returns>An enumeration of templates.</returns>
        IEnumerable<TemplateDefinition> GetTemplates(Type contentType);

        /// <summary>Gets a tmeplate by name and content type.</summary>
        /// <param name="contentType">The type of content item.</param>
        /// <param name="templateKey">The name of the template.</param>
        /// <returns>A matching template or null if no template is available.</returns>
        TemplateDefinition GetTemplate(Type contentType, string templateKey);

		/// <summary>Gets a tmeplate by discriminator/templateKey reference.</summary>
		TemplateDefinition GetTemplate(string discriminatorWithTemplateKey);

        /// <summary>Gets the template of a content item.</summary>
        /// <param name="item">The item whose template to get.</param>
        /// <returns>The item's template.</returns>
        TemplateDefinition GetTemplate(ContentItem item);
    }
}
