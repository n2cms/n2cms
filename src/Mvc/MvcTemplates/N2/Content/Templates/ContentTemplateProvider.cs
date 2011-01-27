using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Edit;
using N2.Definitions;
using N2.Engine;

namespace N2.Management.Content.Templates
{
	[Service(typeof(ITemplateProvider))]
	public class ContentTemplateProvider : ITemplateProvider
	{
		IContentTemplateRepository repository;
		DefinitionBuilder definitionBuilder;
		ItemDefinition[] staticDefinitions = null;

		public ContentTemplateProvider(IContentTemplateRepository repository, DefinitionBuilder definitionBuilder)
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
	}
}