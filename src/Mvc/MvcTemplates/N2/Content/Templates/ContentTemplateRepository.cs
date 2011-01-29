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
	[Service]
	public class ContentTemplateRepository
	{
		public const string TemplateDescription = "TemplateDescription";

		IPersister persister;
		ContainerRepository<TemplateContainer> container;
		IDefinitionManager definitions;

		public ContentTemplateRepository(IPersister persister, ContainerRepository<TemplateContainer> container, IDefinitionManager definitions)
		{
			this.persister = persister;
			this.container = container;
			this.definitions = definitions;
		}

		#region ITemplateRepository Members

		public TemplateDefinition GetTemplate(string templateName)
		{
			TemplateContainer templates = container.GetBelowRoot();
			if (templates == null)
				return null;

			var template = templates.GetChild(templateName);
			return CreateTemplateInfo(template);
		}

		private TemplateDefinition CreateTemplateInfo(ContentItem template)
		{
			var clone = template.Clone(true);
			clone.SetDetail(TemplateDescription, null, typeof(string));
			clone.Title = "";
			clone.Name = null;
			clone["TemplateName"] = template.Name;
			var info = new TemplateDefinition
			{
				Name = template.Name,
				Title = template.Title,
				Description = template.GetDetail(TemplateDescription, ""),
				TemplateUrl = template.Url,
				Definition = definitions.GetDefinition(template.GetContentType()).Clone(),
				Template = clone,
				Original = template
			};
			info.Definition.Template = template.Name;
			return info;
		}

		public IEnumerable<TemplateDefinition> GetAllTemplates()
		{
			TemplateContainer templates = container.GetBelowRoot();
			if (templates == null)
				yield break;

			foreach (ContentItem child in templates.Children)
			{
				yield return CreateTemplateInfo(child);
			}
		}

		public IEnumerable<TemplateDefinition> GetTemplates(Type contentType, IPrincipal user)
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
