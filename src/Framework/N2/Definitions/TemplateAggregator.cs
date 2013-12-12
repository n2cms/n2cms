using N2.Engine;
using N2.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Definitions
{
    [Service(typeof(ITemplateAggregator))]
    public class TemplateAggregator : ITemplateAggregator, IAutoStart
    {
        private ITemplateProvider[] providers;
        private IDefinitionManager definitions;

        public TemplateAggregator(IDefinitionManager definitions, ITemplateProvider[] templateProviders)
        {
            this.definitions = definitions;
            this.providers = templateProviders.OrderBy(tp => tp.SortOrder).ToArray();
        }

        public virtual IEnumerable<TemplateDefinition> GetTemplates(Type contentType)
        {
            if (contentType == null) return new TemplateDefinition[0];

            var templates = providers.SelectMany(tp => tp.GetTemplates(contentType)).ToList();
            if (!templates.Any(t => t.ReplaceDefault))
                return templates;
            return templates.Where(t => t.Name != null).ToList();
        }

        public virtual TemplateDefinition GetTemplate(Type contentType, string templateKey)
        {
            if (contentType == null) return null;

            return providers
                .SelectMany(tp => tp.GetTemplates(contentType))
                .FirstOrDefault(td => string.Equals(td.Name ?? "", templateKey ?? ""));
        }

        public virtual TemplateDefinition GetTemplate(ContentItem item)
        {
            if (item == null) return null;

            return providers.Select(tp => tp.GetTemplate(item)).FirstOrDefault(t => t != null);
        }


        public void Start()
        {
            definitions.DefinitionResolving += definitions_DefinitionResolving;
        }

        public void Stop()
        {
            definitions.DefinitionResolving -= definitions_DefinitionResolving;
        }

        void definitions_DefinitionResolving(object sender, DefinitionEventArgs e)
        {
            if (e.AffectedItem != null)
            {
                var t = GetTemplate(e.AffectedItem);
                if (t != null)
                    e.Definition = t.Definition;
            }
        }
    }
}
