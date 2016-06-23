using N2.Edit;
using N2.Edit.Api;
using N2.Edit.Collaboration;
using N2.Engine;
using N2.Management.Api;
using N2.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Collaboration
{
	[ContentHandler]
	public class CollaborationContentHandler : ContentHandlerBase
	{
		private ContentMessageSource messages;
		private IProfileRepository profiles;
		private IEngine engine;

		public CollaborationContentHandler(IProfileRepository profiles, ContentMessageSource messages, IEngine engine)
		{
			this.profiles = profiles;
			this.messages = messages;
			this.engine = engine;
		}

		public object Messages(HttpContextBase context)
		{
			var selectedItem = new SelectionUtility(context, engine).ParseSelectionFromRequest() ?? engine.Persister.Get(engine.Host.DefaultSite.RootItemID);
			var ctx = CollaborationContext.Create(DateTime.MinValue, selectedItem, context.User);
			return new
			{
				Messages = messages.GetMessages(ctx).ToList()
			};
		}

		public object Notes(HttpContextBase context, int? skip, int? take)
		{
			var item = new SelectionUtility(context, engine).ParseSelectionFromRequest();
			if (item != null)
			{
				return new
				{
					Notes = item["CollaborationNote"] != null ? new[] { item["CollaborationNote"] } : new object[0]
				};
			}
			else
			{
				var items = engine.Persister.Repository.Find(Parameter.IsNotNull("CollaborationNote").Detail().Skip(skip ?? 0).Take(take ?? 1000));
				return new
				{
					AnnotatedItems = items.Select(ci => new { ci.ID, ci.Title, ci.Url, ci.IconUrl, ci.IconClass, Notes = new [] { ci["CollaborationNote"] } }).ToList()
				};
			}
		}

		public class PostNotesDto
		{
			public string note;
		}

		public object PostNotes(HttpContextBase context, Dictionary<string, object> body)
		{
			var item = new SelectionUtility(context, engine).SelectedItem;
			item["CollaborationNote"] = body["note"];
			engine.Persister.Save(item);

			return new { Success = true };
		}

	}
}