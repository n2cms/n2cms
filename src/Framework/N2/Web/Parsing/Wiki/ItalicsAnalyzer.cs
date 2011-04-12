using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Parsing.Wiki
{
	public class ItalicsAnalyzer : StartStopAnalyzerBase
	{
		public ItalicsAnalyzer()
			: base("''")
		{
			AllowMarkup = true;
		}
	}
}
