using N2.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit.Collaboration
{
	[Service]
	public class ManagementMessageCollector : IMessageSource
	{
		private IEnumerable<IMessageSource> sources;

		public ManagementMessageCollector(IMessageSource[] sources)
		{
			this.sources = sources;
		}

		public virtual IEnumerable<CollaborationMessage> GetMessages(CollaborationContext context)
		{
			return sources.SelectMany(s => s.GetMessages(context));
		}
	}
}
