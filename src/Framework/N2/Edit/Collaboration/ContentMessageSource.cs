using N2.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit.Collaboration
{
	[Service(typeof(IMessageSource))]
	public class ContentMessageSource : IMessageSource
	{
		public IEnumerable<CollaborationMessage> GetMessages(CollaborationContext context)
		{
			if (context.SelectedItem == null)
				return CollaborationMessage.None;

			return Content.Traverse.Ancestors(context.SelectedItem, lastAncestor: null)
				.OfType<IMessageSource>()
				.SelectMany(ms => ms.GetMessages(context));
		}
	}
}
