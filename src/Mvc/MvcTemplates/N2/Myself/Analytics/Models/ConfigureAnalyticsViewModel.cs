using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Reimers.Google.Analytics;
using System.Web.Mvc;

namespace N2.Management.Myself.Analytics.Models
{
    public class ConfigureAnalyticsViewModel
    {
        public List<Dimension> AllDimensions { get; set; }
        public List<Metric> AllMetrics { get; set; }
        public List<Dimension> SelectedDimensions { get; set; }
        public List<Metric> SelectedMetrics { get; set; }
        public int Period { get; set; }
        public SelectListItem[] Periods { get; set; }
    }
}
