using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Parsing.Wiki
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
