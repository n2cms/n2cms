using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit
{
	public class TreeNode
	{
		public string Title { get; set; }
		public string ToolTip { get; set; }

		public string PreviewUrl { get; set; }
		public string Target { get; set; }
		public string CssClass { get; set; }

		public string IconUrl { get; set; }

		public Security.Permission MaximumPermission { get; set; }

		public IEnumerable<KeyValuePair<string, string>> MetaInforation { get; set; }
	}
}
