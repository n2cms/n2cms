using N2.Engine;
using N2.Management.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace N2.Edit.Collaboration
{
	[Service]
	public class ManagementMessageCollector : IMessageSource
	{
		private IProfileRepository profiles;
		private IEnumerable<IMessageSource> sources;

		public ManagementMessageCollector(IProfileRepository profiles, IMessageSource[] sources)
		{
			this.profiles = profiles;
			this.sources = sources;
		}

		public virtual IEnumerable<CollaborationMessage> GetMessages(CollaborationContext context)
		{
			var messages = sources.SelectMany(s => s.GetMessages(context));
			if (context.LastDismissed > DateTime.MinValue)
				messages = messages.Where(m => m.Updated > context.LastDismissed);
			return messages;
		}
	}
}
