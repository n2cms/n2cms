using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web.Parsing;

namespace N2.Web.Wiki.Analyzers
{
	public class H3Analyzer : StartStopAnalyzerBase
	{
		public H3Analyzer()
			: base("===")
		{
			AllowMarkup = true;
		}
	}
}
