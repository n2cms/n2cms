using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit.Collaboration
{
	public class CollaborationContext
	{
		public ContentItem SelectedItem { get; set; }
		public DateTime LastDismissed { get; set; }
	}
}
