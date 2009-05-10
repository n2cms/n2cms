using System.Collections.Generic;

namespace N2.Addons.Tagging.Details
{
	public class AppliedTags
	{
		public ITagCategory Category { get; set; }
		public IList<string> Tags { get; set; }
	}
}