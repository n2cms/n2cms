using System;
using System.Collections.Specialized;
using System.Web;
using N2.Definitions;
using N2.Edit;
using N2.Persistence;
using N2.Web;

namespace N2.Parts.Web
{
	public class CreateUrlProvider : PartsAjaxService
	{
		private readonly IPersister persister;
		private readonly IEditManager editManager;
		private readonly IDefinitionManager definitions;

		public CreateUrlProvider(IPersister persister, IEditManager editManager, IDefinitionManager definitions, AjaxRequestDispatcher dispatcher)
			: base(dispatcher)
		{
			this.persister = persister;
			this.editManager = editManager;
			this.definitions = definitions;
		}

		public override string Name
		{
			get { return "create"; }
		}

		public override NameValueCollection Handle(NameValueCollection request)
		{
			NameValueCollection response = new NameValueCollection();
			response["url"] = GetRedirectUrl(request); ;
			return response;
		}

		private string GetRedirectUrl(NameValueCollection request)
		{
			string zone = request["zone"];

			string before = request["before"];
			string below = request["below"];
			string discriminator = request["discriminator"];
			ItemDefinition definition = GetDefinition(discriminator);

			string url;
			if (!string.IsNullOrEmpty(below))
			{
				ContentItem parent = persister.Get(int.Parse(below));
				url = editManager.GetEditNewPageUrl(parent, definition, zone, CreationPosition.Below);
			}
			else
			{
				ContentItem beforeItem = Context.Persister.Get(int.Parse(before));
				url = editManager.GetEditNewPageUrl(beforeItem, definition, zone, CreationPosition.Before);
			}

			url = Utility.ToAbsolute(url);
			if (!string.IsNullOrEmpty(request["returnUrl"]))
				url += "&returnUrl=" + HttpUtility.UrlEncode(request["returnUrl"]);
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