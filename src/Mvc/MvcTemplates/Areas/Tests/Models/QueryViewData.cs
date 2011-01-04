#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Templates.Mvc.Areas.Tests.Models
{
	public class QueryViewData
	{
		public Func<List<string>> Queries { get; set; }
		public Func<List<string>> All { get; set; }
	}
}
#endif