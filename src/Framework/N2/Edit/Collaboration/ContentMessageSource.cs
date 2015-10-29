using N2.Engine;
using N2.Integrity;
using N2.Persistence;
using N2.Security;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit.Collaboration
{
	[Service]
	[MessageSource]
	public class ContentMessageSource : MessageSourceBase
	{
		private IPersister persister;
		private IIntegrityManager integrity;
		private ISecurityManager security;
		private IWebContext context;

		public ContentMessageSource(IPersister persister, IIntegrityManager integrity, ISecurityManager security, IWebContext context)
		{
			this.persister = persister;
			this.integrity = integrity;
			this.security = security;
			this.context = context;
		}

		public override IEnumerable<CollaborationMessage> GetMessages(CollaborationContext context)
		{
			if (context.SelectedItem == null)
				return CollaborationMessage.None;

			return Find.EnumerateParents(context.SelectedItem, null, includeSelf: true)
				.OfType<IMessageSource>()
				.SelectMany(ms => ms.GetMessages(context));
		}

		public override void Delete(string sourceName, string messageID)
		{
			int id;
			if (int.TryParse(messageID, out id))
			{
				var item = persister.Get(id);
				if (item is IMessageSource)
				{
					var ex = integrity.GetDeleteException(item);
					if (ex != null)
						throw ex;

					if (!security.IsAuthorized(context.User, item, item.IsPublished() ? Security.Permission.Publish : Security.Permission.Write))
						throw new UnauthorizedAccessException();

					persister.Delete(item);
				}

			}
		}

		public override SourceInfo GetInfo()
		{
			return new SourceInfo { Name = GetType().Name, SupportsDelete = true };
		}
	}
}
