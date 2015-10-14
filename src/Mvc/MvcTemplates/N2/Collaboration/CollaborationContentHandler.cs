using N2.Edit;
using N2.Edit.Api;
using N2.Edit.Collaboration;
using N2.Engine;
using N2.Management.Api;
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
			var ctx = CollaborationContext.Create(DateTime.MinValue, new SelectionUtility(context, engine).SelectedItem ?? engine.UrlParser.StartPage, context.User);
			return new
			{
				Messages = messages.GetMessages(ctx).ToList()
			};
		}
	}
}