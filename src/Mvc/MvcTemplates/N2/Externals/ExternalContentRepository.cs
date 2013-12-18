using System;
using N2.Configuration;
using N2.Edit;
using N2.Engine;
using N2.Persistence;
using N2.Web;

namespace N2.Management.Externals
{
    [Service(typeof(IExternalContentRepository))]
    public class ExternalContentRepository : IExternalContentRepository
    {
        ContainerRepository<ExternalItem> containerRepository;
        IPersister persister;
        ContentActivator activator;
        Type externalItemType;

        public ExternalContentRepository(ContainerRepository<ExternalItem> containerRepository, IPersister persister, ContentActivator activator, EditSection config)
        {
            this.containerRepository = containerRepository;
            this.persister = persister;
            this.activator = activator;
            externalItemType = Type.GetType(config.Externals.ExternalItemType) ?? typeof(ExternalItem);
        }

        public ContentItem GetOrCreate(string familyKey, string key, string url, Type contentType = null)
        {
            var container = containerRepository.GetOrCreateBelowStart((ei) => 
            {
                ei.Visible = false;
                ei.TemplateKey = ExternalItem.ContainerTemplateKey;
                ei.Title = ExternalItem.ExternalContainerName;
                ei.Name = ExternalItem.ExternalContainerName;
            });
            var familyContainer = containerRepository.GetOrCreate(container, (ei) =>
            {
                ei.Visible = false;
                ei.TemplateKey = ExternalItem.ContainerTemplateKey;
                ei.Title = familyKey;
                ei.Name = familyKey;
                ei.ZoneName = ExternalItem.ExternalContainerName;
            }, name: familyKey);

            if (string.IsNullOrEmpty(key))
                key = ExternalItem.SingleItemKey;

            var item = familyContainer.Children.FindNamed(key)
                ?? Create(familyKey, key, url, familyContainer, contentType);
            return item;
        }

        private ContentItem Create(string family, string key, Url url, ContentItem container, Type contentType)
        {
            string externalUrl = url.RemoveQuery("edit").ToString();
            var item = activator.CreateInstance(contentType ?? externalItemType, container);
            item.Title = "";
            item.Name = key;
            item.ZoneName = family;
            item["ExternalUrl"] = externalUrl;
            persister.Save(item);
            return item;
        }
    }
}
