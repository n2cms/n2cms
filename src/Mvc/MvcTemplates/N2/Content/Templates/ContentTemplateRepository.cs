using System;
using System.Collections.Generic;
using System.Security.Principal;
using N2.Definitions;
using N2.Definitions.Static;
using N2.Edit;
using N2.Engine;
using N2.Persistence;
using System.Linq;

namespace N2.Management.Content.Templates
{
    [Service]
    public class ContentTemplateRepository
    {
        public const string TemplateDescription = "TemplateDescription";

        private IRepository<ContentItem> repository;
        private DefinitionMap map;
        private ContainerRepository<TemplateContainer> container;

        public ContentTemplateRepository(IRepository<ContentItem> repository, DefinitionMap map, ContainerRepository<TemplateContainer> container)
        {
            this.repository = repository;
            this.map = map;
            this.container = container;
        }

        #region ITemplateRepository Members

        public TemplateDefinition GetTemplate(string templateKey)
        {
            TemplateContainer templates = container.GetBelowRoot();
            if (templates == null)
                return null;

            var template = templates.GetChild(templateKey);
            return CreateTemplateInfo(template);
        }

        private TemplateDefinition CreateTemplateInfo(ContentItem template)
        {
            var info = new TemplateDefinition
            {
                Name = template.Name,
                Title = template.Title,
                Description = template.GetDetail(TemplateDescription, ""),
                TemplateUrl = template.Url,
                Definition = map.GetOrCreateDefinition(template.GetContentType(), template.Name),
                TemplateFactory = () =>
                {
                    var clone = template.Clone(true);
                    clone.SetDetail(TemplateDescription, null, typeof(string));
                    clone.Title = "";
                    clone.Name = null;
                    clone.TemplateKey = template.Name;
                    return clone;
                },
                OriginalFactory = () => template
            };
            return info;
        }

        public IEnumerable<TemplateDefinition> GetAllTemplates()
        {
            TemplateContainer templates = container.GetBelowRoot();
			if (templates == null)
				return new TemplateDefinition[0];

			return templates.Children.Select(t => CreateTemplateInfo(t)).ToArray();
        }

        public IEnumerable<TemplateDefinition> GetTemplates(Type contentType, IPrincipal user)
        {
            foreach(var template in GetAllTemplates())
            {
                if (template.Definition.ItemType != contentType)
                    continue;
                if (!template.TemplateFactory().IsAuthorized(user))
                    continue;

                yield return template;
            }
        }

        public void AddTemplate(ContentItem templateItem)
        {
            TemplateContainer templates = container.GetOrCreateBelowRoot((c) =>
                {
                    c.Title = "Templates";
                    c.Name = "Templates";
                    c.Visible = false;
                    c.SortOrder = int.MaxValue - 1001000;
                });

            templateItem.Name = null;
            templateItem.AddTo(templates);

			using (var tx = repository.BeginTransaction())
			{
				repository.SaveOrUpdate(templateItem);
				tx.Commit();
			}
        }

        public void RemoveTemplate(string templateKey)
        {
            TemplateContainer templates = container.GetBelowRoot();
            if (templates == null)
                return;

            ContentItem template = templates.GetChild(templateKey);
            if (template == null)
                return;

			using(var tx = repository.BeginTransaction())
			{
				template.AddTo(null);
				repository.Delete(template);
				tx.Commit();
			}
        }

        #endregion
    }
}
