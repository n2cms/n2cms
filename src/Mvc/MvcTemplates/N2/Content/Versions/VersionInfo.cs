using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Management.Content.Versions
{
	public class VersionInfo
	{
		public string Info { get; set; }

		public string SavedBy { get; set; }

		public string Title { get; set; }

		public int? VersionIndex { get; set; }

		public int ID { get; set; }

		public bool InProgress { get; set; }

		public ContentState State { get; set; }
	}
}
