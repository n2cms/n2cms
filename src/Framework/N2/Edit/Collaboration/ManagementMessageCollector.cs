using N2.Engine;
using N2.Management.Api;
using N2.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace N2.Edit.Collaboration
{
	[Service]
	public class ManagementMessageCollector : MessageSourceBase
	{
		private IProfileRepository profiles;
		private ISecurityManager security;
		private IEnumerable<MessageSourceBase> sources;

		public ManagementMessageCollector(IProfileRepository profiles, ISecurityManager security, MessageSourceBase[] sources)
		{
			this.profiles = profiles;
			this.security = security;
			this.sources = sources;
		}

		public override IEnumerable<CollaborationMessage> GetMessages(CollaborationContext context)
		{
			var messages = sources.SelectMany(s => s.GetMessages(context).Select(m => new { m, s }));
			if (context.LastDismissed > DateTime.MinValue)
				messages = messages.Where(x => x.m.Updated > context.LastDismissed);
			messages = messages.Where(m => security.IsAuthorized(context.User, m.m.RequiredPermission));
			return messages.Select(x => 
			{ 
				x.m.Source = x.s.GetInfo();
				if (x.m.Source.SupportsDelete && !security.IsAdmin(context.User))
					x.m.Source.SupportsDelete = false;
				return x.m;
			});
		}

		public override void Delete(string sourceName, string messageID)
		{
			var source = sources.FirstOrDefault(s => s.GetInfo().Name == sourceName);
			if (source != null)
				source.Delete(sourceName, messageID);
		}
	}
}
