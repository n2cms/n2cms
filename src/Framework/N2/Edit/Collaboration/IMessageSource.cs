using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit.Collaboration
{
	public interface IMessageSource
	{
		IEnumerable<CollaborationMessage> GetMessages(CollaborationContext context);
	}
}
