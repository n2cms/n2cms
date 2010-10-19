using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions;
using System.Security.Principal;

namespace N2.Edit.Templating
{
	public interface ITemplateRepository
	{
		TemplateInfo GetTemplate(string templateName);
		IEnumerable<TemplateInfo> GetAllTemplates();
		IEnumerable<TemplateInfo> GetTemplates(Type contentType, IPrincipal User);
		void AddTemplate(ContentItem templateItem);
		void RemoveTemplate(string templateName);
	}
}
