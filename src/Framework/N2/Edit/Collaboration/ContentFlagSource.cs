using N2.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit.Collaboration
{
	[Service(typeof(IFlagSource))]
	public class ContentFlagSource : IFlagSource
	{
		public IEnumerable<string> GetFlags(CollaborationContext context)
		{
			if (context.SelectedItem == null)
				return new string[0];

			return N2.Find.EnumerateParents(context.SelectedItem, lastAncestor: null, includeSelf: true)
				.OfType<IFlagSource>()
				.SelectMany(ms => ms.GetFlags(context));
		}
	}
}
