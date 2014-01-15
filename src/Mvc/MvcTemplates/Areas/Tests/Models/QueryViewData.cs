#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Templates.Mvc.Areas.Tests.Controllers;

namespace N2.Templates.Mvc.Areas.Tests.Models
{
    public class QueryViewData
    {
        public Func<List<Row>> Queries { get; set; }
        public Func<List<Row>> All { get; set; }
    }
}
#endif
