using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Plugin;
using N2.Persistence;

namespace N2.Edit.Versioning
{
	public class ContentVersionCleanup : IAutoStart
	{
		public ContentVersionCleanup(IItemNotifier notifier, ContentVersionRepository repository)
		{
		}

		public void Start()
		{
		}

		public void Stop()
		{
		}
	}
}
