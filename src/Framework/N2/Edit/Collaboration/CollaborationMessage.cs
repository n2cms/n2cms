using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit.Collaboration
{
	public class CollaborationMessage
	{
		public static IEnumerable<CollaborationMessage> None = new CollaborationMessage[0];

		public string ID { get; set; }
		public string Title { get; set; }
		public bool Alert { get; set; }
		public string Text { get; set; }
		public DateTime Updated { get; set; }
		public SourceInfo Source { get; set; }
		public Security.Permission RequiredPermission { get; set; }
	}
}
