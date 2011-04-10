using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Edit;
using N2.Definitions;
using N2.Engine;
using N2.Definitions.Static;

namespace N2.Management.Content.Templates
{
	[Service(typeof(ITemplateProvider))]
	public class ContentTemplateProvider : ITemplateProvider
	{
		ContentTemplateRepository repository;
		DefinitionBuilder definitionBuilder;

		public ContentTemplateProvider(ContentTemplateRepository repository, DefinitionBuilder definitionBuilder)
		{
		    this.repository = repository;
		    this.definitionBuilder = definitionBuilder;
		}

		#region ITemplateProvider Members

		public IEnumerable<TemplateDefinition> GetTemplates(Type contentType)
		{
			return repository.GetAllTemplates().Where(t => t.Definition.ItemType == contentType);
		}

		#endregion

		#region ITemplateProvider Members

		public TemplateDefinition GetTemplate(ContentItem item)
		{
			string templateName = item["TemplateName"] as string;
			if(templateName == null)
				return null;

			return GetTemplates(item.GetContentType()).Where(t => t.Name == templateName).Select(t =>
			{
				t.Original = t.Template;
				t.Template = () => item;
				return t;
			}).FirstOrDefault();
		}

		#endregion
	}
}