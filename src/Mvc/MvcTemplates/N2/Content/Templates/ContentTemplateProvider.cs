using System;
using System.Collections.Generic;
using System.Linq;
using N2.Definitions;
using N2.Definitions.Static;
using N2.Engine;

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
            string templateKey = item.TemplateKey;
            if(templateKey == null)
                return null;

            return GetTemplates(item.GetContentType()).Where(t => t.Name == templateKey).Select(t =>
            {
                t.OriginalFactory = t.TemplateFactory;
                t.TemplateFactory = () => item;
                return t;
            }).FirstOrDefault();
        }

        #endregion

        /// <summary>The order this template provider should be invoked, default 0.</summary>
        public int SortOrder { get; set; }
    }
}
