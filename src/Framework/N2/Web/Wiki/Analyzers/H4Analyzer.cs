using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web.Parsing;

namespace N2.Web.Wiki.Analyzers
{
	public class H4Analyzer : StartStopAnalyzerBase
	{
		public H4Analyzer()
			: base("====")
		{
			AllowMarkup = true;
		}
	}
}
