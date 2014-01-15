using System.Collections.Specialized;
using N2.Definitions;
using N2.Edit;
using N2.Engine;
using N2.Persistence;
using N2.Edit.Versioning;
using System;

namespace N2.Web.Parts
{
    [Service(typeof(IAjaxService))]
    public class CreateUrlProvider : PartsAjaxService
    {
        readonly IPersister persister;
        readonly IEditUrlManager managementPaths;
        readonly ContentActivator activator;
        readonly IDefinitionManager definitions;
        readonly ITemplateAggregator templates;
        readonly Navigator navigator;
        private IVersionManager versions;
        private ContentVersionRepository versionRepository;

        public CreateUrlProvider(IPersister persister, IEditUrlManager editUrlManager, IDefinitionManager definitions, ITemplateAggregator templates, ContentActivator activator, Navigator navigator, IVersionManager versions, ContentVersionRepository versionRepository)
        {
            this.persister = persister;
            this.managementPaths = editUrlManager;
            this.definitions = definitions;
            this.templates = templates;
            this.activator = activator;
            this.navigator = navigator;
            this.versions = versions;
            this.versionRepository = versionRepository;
        }

        public override string Name
        {
            get { return "create"; }
        }

        public override NameValueCollection HandleRequest(NameValueCollection request)
        {
            NameValueCollection response = new NameValueCollection();

            var template = GetTemplate(request["discriminator"], request["template"]);
            if (template.Definition.Editables.Count > 0)
            {
                response["redirect"] = GetRedirectUrl(template, request);
                response["dialog"] = "yes";
            }
            else
            {
                response["dialog"] = "no";
                response["redirect"] = CreateItem(template, request);
            }

            return response;
        }

        private string CreateItem(TemplateDefinition template, NameValueCollection request)
        {
            var path = new PathData(navigator.Navigate(request["below"]));
            if (!versionRepository.TryParseVersion(request[PathData.VersionIndexQueryKey], request["versionKey"], path))
                path.CurrentItem = versions.AddVersion(path.CurrentItem, asPreviousVersion: false);
            var parent = path.CurrentItem;

            ContentItem item = activator.CreateInstance(template.Definition.ItemType, parent);
            item.ZoneName = request["zone"];
            item.TemplateKey = template.Name;

            string beforeVersionKey = request["beforeVersionKey"];
            string beforeSortOrder = request["beforeSortOrder"];
            string before = request["before"];
            if (string.IsNullOrEmpty(beforeSortOrder))
            {
                item.AddTo(parent);
            }
            else
            {
                int index = int.Parse(beforeSortOrder);
                parent.InsertChildBefore(item, index);
            }

            versionRepository.Save(parent);
            return request["returnUrl"].ToUrl().SetQueryParameter(PathData.VersionIndexQueryKey, parent.VersionIndex);
        }

        private string GetRedirectUrl(TemplateDefinition template, NameValueCollection request)
        {
            string zone = request["zone"];

            string before = request["before"];
            string below = request["below"];

            Url url = null;
            if (!string.IsNullOrEmpty(before))
            {
                ContentItem beforeItem = navigator.Navigate(before);
                if (beforeItem != null)
                    url = managementPaths.GetEditNewPageUrl(beforeItem, template.Definition, zone, CreationPosition.Before);
            }
            if (url == null)
            {
                ContentItem parent = navigator.Navigate(below);
                url = managementPaths.GetEditNewPageUrl(parent, template.Definition, zone, CreationPosition.Below);
            }
            string beforeSortOrder = request["beforeSortOrder"];
            url = url.SetQueryParameter("beforeSortOrder", beforeSortOrder);

            if (!string.IsNullOrEmpty(request[PathData.VersionIndexQueryKey]))
                url = url.SetQueryParameter(PathData.VersionIndexQueryKey, request[PathData.VersionIndexQueryKey]);
            
            //if (!string.IsNullOrEmpty(request["versionKey"]))
            //    url = url.SetQueryParameter("versionKey", request["versionKey"]);
            if (!string.IsNullOrEmpty(request["belowVersionKey"]))
                url = url.SetQueryParameter("versionKey", request["belowVersionKey"]);
            if (!string.IsNullOrEmpty(request["belowVersionKey"]))
                url = url.SetQueryParameter("versionKey", request["belowVersionKey"]);
            if (!string.IsNullOrEmpty(request["beforeVersionKey"]))
                url = url.SetQueryParameter("beforeVersionKey", request["beforeVersionKey"]);
    
            if (!string.IsNullOrEmpty(request["returnUrl"]))
                url = url.SetQueryParameter("returnUrl", request["returnUrl"]);

            url = url.SetQueryParameter("edit", "drag");
            return url;
        }

        private TemplateDefinition GetTemplate(string discriminator, string templateKey)
        {
            return templates.GetTemplate(definitions.GetDefinition(discriminator).ItemType, templateKey);
        }
    }
}
