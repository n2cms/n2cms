using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions;
using System.Security.Principal;

namespace N2.Edit
{
	public interface IContentTemplateRepository
	{
		TemplateDefinition GetTemplate(string templateName);
		IEnumerable<TemplateDefinition> GetAllTemplates();
		IEnumerable<TemplateDefinition> GetTemplates(Type contentType, IPrincipal User);
		void AddTemplate(ContentItem templateItem);
		void RemoveTemplate(string templateName);
	}
}
