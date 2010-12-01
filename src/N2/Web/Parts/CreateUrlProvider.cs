using System;
using System.Collections.Specialized;
using System.Web;
using N2.Definitions;
using N2.Edit;
using N2.Persistence;
using N2.Web;
using N2.Engine;

namespace N2.Web.Parts
{
	[Service]
	public class CreateUrlProvider : PartsAjaxService
	{
        readonly IPersister persister;
		readonly IEditUrlManager editUrlManager;
		readonly IDefinitionManager definitions;
        readonly Navigator navigator;

		public CreateUrlProvider(IPersister persister, IEditUrlManager editUrlManager, IDefinitionManager definitions, AjaxRequestDispatcher dispatcher, Navigator navigator)
			: base(dispatcher)
		{
            this.persister = persister;
			this.editUrlManager = editUrlManager;
			this.definitions = definitions;
            this.navigator = navigator;
		}

		public override string Name
		{
			get { return "create"; }
		}

		public override NameValueCollection HandleRequest(NameValueCollection request)
		{
			NameValueCollection response = new NameValueCollection();

            ItemDefinition definition = GetDefinition(request["discriminator"]);
            if (definition.Editables.Count > 0)
                response["redirect"] = GetRedirectUrl(definition, request);
            else
            {
                response["redirect"] = request["returnUrl"];
                CreateItem(definition, request);
            }

			return response;
		}

        private void CreateItem(ItemDefinition definition, NameValueCollection request)
        {
            ContentItem parent = navigator.Navigate(request["below"]);

            ContentItem item = definitions.CreateInstance(definition.ItemType, parent);
            item.ZoneName = request["zone"];
            
            string before = request["before"];
            if (!string.IsNullOrEmpty(before))
			{
				ContentItem beforeItem = navigator.Navigate(before);
                int newIndex = parent.Children.IndexOf(beforeItem);
                Utility.Insert(item, parent, newIndex);

                foreach (var sibling in Utility.UpdateSortOrder(parent.Children))
                    persister.Repository.Save(sibling);
			}

            persister.Save(item);
        }

		private string GetRedirectUrl(ItemDefinition definition, NameValueCollection request)
		{
			string zone = request["zone"];

			string before = request["before"];
			string below = request["below"];

			Url url;
			if (!string.IsNullOrEmpty(before))
			{
                ContentItem beforeItem = navigator.Navigate(before);
                url = editUrlManager.GetEditNewPageUrl(beforeItem, definition, zone, CreationPosition.Before);
			}
			else
			{
                ContentItem parent = navigator.Navigate(below);
                url = editUrlManager.GetEditNewPageUrl(parent, definition, zone, CreationPosition.Below);
			}

			if (!string.IsNullOrEmpty(request["returnUrl"]))
				url = url.AppendQuery("returnUrl", request["returnUrl"]);
			return url;
		}

		private ItemDefinition GetDefinition(string discriminator)
		{
			foreach (ItemDefinition definition in definitions.GetDefinitions())
			{
				if (definition.Discriminator == discriminator)
				{
					return definition;
				}
			}
			throw new N2Exception("Definition not found: " + discriminator);
		}
	}
}