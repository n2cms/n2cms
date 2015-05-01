using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit.Collaboration
{
	public abstract class MessageSourceBase : IMessageSource
	{
		public abstract IEnumerable<CollaborationMessage> GetMessages(CollaborationContext context);
		
		public virtual void Delete(string sourceName, string messageID)
		{
		}

		public virtual SourceInfo GetInfo()
		{
			return new SourceInfo { Name = GetType().Name, SupportsDelete = false };
		}
	}
}
