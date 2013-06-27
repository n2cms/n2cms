using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace N2.Web.Targeting
{
	public class TargetingContext
	{
		public TargetingContext(HttpContextBase context)
		{
			HttpContext = context;
			Flags = new HashSet<string>();
		}

		public HttpContextBase HttpContext { get; set; }
		public HashSet<string> Flags { get; set; }
	}
}
