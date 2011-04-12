using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Parsing.Wiki
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
