using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Engine;
using N2.Web;
using N2.Persistence;
using N2;
using N2.Definitions;
using N2.Details;
using N2.Edit;
using System.Security.Principal;

namespace N2.Management.Content.Templates
{
	[Service(typeof(IContentTemplateRepository))]
	public class ContentTemplateRepository : IContentTemplateRepository
	{
		public const string TemplateDescription = "TemplateDescription";

		IPersister persister;
		IDefinitionManager definitions;
		ContainerRepository<TemplateContainer> container;

		public ContentTemplateRepository(IPersister persister, IDefinitionManager definitions, ContainerRepository<TemplateContainer> container)
		{
			this.persister = persister;
			this.definitions = definitions;
			this.container = container;
		}

		#region ITemplateRepository Members

		public ContentTemplate GetTemplate(string templateName)
		{
			TemplateContainer templates = container.GetBelowRoot();
			if (templates == null)
				return null;

			var template = templates.GetChild(templateName);
			return CreateTemplateInfo(template);
		}

		private ContentTemplate CreateTemplateInfo(ContentItem template)
		{
			var clone = template.Clone(true);
			var info = new ContentTemplate
			{
				Name = template.Name,
				Title = template.Title,
				Description = template.GetDetail(TemplateDescription, ""),
				TemplateUrl = template.Url,
				Definition = definitions.GetDefinition(template.GetContentType()),
				Template = clone,
				Original = template
				//HiddenEditors = (template.GetDetailCollection("HiddenEditors", false) ?? new DetailCollection()).ToList<string>(),
			};
			clone.SetDetail(TemplateDescription, null, typeof(string));
			clone.Title = "";
			clone.Name = null;
			return info;
		}

		public IEnumerable<ContentTemplate> GetAllTemplates()
		{
			TemplateContainer templates = container.GetBelowRoot();
			if (templates == null)
				yield break;

			foreach (ContentItem child in templates.Children)
			{
				yield return CreateTemplateInfo(child);
			}
		}

		public IEnumerable<ContentTemplate> GetTemplates(Type contentType, IPrincipal user)
		{
			foreach(var template in GetAllTemplates())
			{
				if (template.Template.GetContentType() != contentType)
					continue;
				if (!template.Template.IsAuthorized(user))
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
			persister.Save(templateItem);
		}

		public void RemoveTemplate(string templateName)
		{
			TemplateContainer templates = container.GetBelowRoot();
			if (templates == null)
				return;

			ContentItem template = templates.GetChild(templateName);
			if (template == null)
				return;

			persister.Delete(template);			
		}

		#endregion
	}
}
