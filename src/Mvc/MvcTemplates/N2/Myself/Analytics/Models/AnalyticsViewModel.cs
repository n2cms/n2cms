using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Reimers.Google.Analytics;

namespace N2.Management.Myself.Analytics.Models
{
    public class AnalyticsViewModel
    {
        public IEnumerable<GenericEntry> Entries { get; set; }
    }
}
