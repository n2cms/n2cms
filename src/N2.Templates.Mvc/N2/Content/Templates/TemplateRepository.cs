using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Edit.Templating;
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
	[Service(typeof(ITemplateRepository))]
	public class TemplateRepository : ITemplateRepository
	{
		public const string TemplateDescription = "TemplateDescription";

		IPersister persister;
		IDefinitionManager definitions;
		ContainerRepository<TemplateContainer> container;

		public TemplateRepository(IPersister persister, IDefinitionManager definitions, ContainerRepository<TemplateContainer> container)
		{
			this.persister = persister;
			this.definitions = definitions;
			this.container = container;
		}

		#region ITemplateRepository Members

		public TemplateInfo GetTemplate(string templateName)
		{
			TemplateContainer templates = container.GetBelowRoot();
			if (templates == null)
				return null;

			var template = templates.GetChild(templateName);
			return CreateTemplateInfo(template);
		}

		private TemplateInfo CreateTemplateInfo(ContentItem template)
		{
			template = template.Clone(true);
			var info = new TemplateInfo
			{
				Name = template.Name,
				Title = template.Title,
				Description = template.GetDetail(TemplateDescription, ""),
				Definition = definitions.GetDefinition(template.GetContentType()),
				Template = template
				//HiddenEditors = (template.GetDetailCollection("HiddenEditors", false) ?? new DetailCollection()).ToList<string>(),
			};
			template.SetDetail(TemplateDescription, null, typeof(string));
			template.Title = "";
			template.Name = null;
			return info;
		}

		public IEnumerable<TemplateInfo> GetAllTemplates()
		{
			TemplateContainer templates = container.GetBelowRoot();
			if (templates == null)
				yield break;

			foreach (ContentItem child in templates.Children)
			{
				yield return CreateTemplateInfo(child);
			}
		}

		public IEnumerable<TemplateInfo> GetTemplates(Type contentType, IPrincipal user)
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
