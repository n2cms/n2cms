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
		Type externalItemType;

		public ExternalContentRepository(ContainerRepository<ExternalItem> containerRepository, IPersister persister, EditSection config)
		{
			this.containerRepository = containerRepository;
			this.persister = persister;
			externalItemType = Type.GetType(config.Externals.ExternalItemType) ?? typeof(ExternalItem);
		}

		public ContentItem GetOrCreate(string familyKey, string key, string url)
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
			});

			if (string.IsNullOrEmpty(key))
				key = ExternalItem.SingleItemKey;

			var item = familyContainer.Children.FindNamed(key)
				?? Create(familyKey, key, url, familyContainer);
			return item;
		}

		private ExternalItem Create(string family, string key, Url url, ExternalItem container)
		{
			string externalUrl = url.RemoveQuery("edit").ToString();
			var item = new ExternalItem { Title = "", Name = key, ZoneName = family, Parent = container };
			item["ExternalUrl"] = externalUrl;
			persister.Save(item);
			return item;
		}
	}
}
