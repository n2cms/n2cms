using N2.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit.Collaboration
{
	[Service]
	public class ManagementFlagCollector : IFlagSource
	{
		private IEnumerable<IFlagSource> sources;

		public ManagementFlagCollector(IFlagSource[] sources)
		{
			this.sources = sources;
		}

		public virtual IEnumerable<string> GetFlags(CollaborationContext context)
		{
			return sources.SelectMany(f => f.GetFlags(context));
		}
	}
}
