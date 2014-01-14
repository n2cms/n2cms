using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Integrity;
using N2.Definitions;
using N2.Details;

namespace N2.Management.Myself.Analytics.Models
{
    [PartDefinition("Analytics",
        IconUrl = "{ManagementUrl}/Resources/icons/chart_pie.png")]
    [RestrictParents(typeof(IRootPage))]
    [Disable]
    public class ManageAnalyticsPart : AnalyticsPartBase
    {
        [EditableText("Token", 100)]
        public virtual string Token
        {
            get { return (string)GetDetail("Token"); }
            set { SetDetail("Token", value, string.Empty); }
        }

        [EditableText("AccountID", 100)]
        public virtual int AccountID
        {
            get { return GetDetail("AccountID", 0); }
            set { SetDetail("AccountID", value, 0); }
        }

        [EditableText("ProfileID", 100)]
        public virtual int ProfileID
        {
            get { return GetDetail("ProfileID", 0); }
            set { SetDetail("ProfileID", value, 0); }
        }

        [EditableText("AccountName", 100)]
        public virtual string AccountName
        {
            get { return GetDetail("AccountName", ""); }
            set { SetDetail("AccountName", value, ""); }
        }

        [EditableText("ChartPeriod", 100)]
        public virtual int ChartPeriod
        {
            get { return GetDetail("ChartPeriod", 31); }
            set { SetDetail("ChartPeriod", value, 31); }
        }

        //public virtual IEnumerable<Dimension> Dimensions
        //{
        //    get { return GetDetailCollection("Dimensions", true).OfType<int>().Select(i => (Dimension)i); }
        //    set { GetDetailCollection("Dimensions", true).Replace(value.Select(d => (int)d)); }
        //}

        //public virtual IEnumerable<Metric> Metrics
        //{
        //    get { return GetDetailCollection("Metrics", true).OfType<int>().Select(i => (Metric)i); }
        //    set { GetDetailCollection("Metrics", true).Replace(value.Select(d => (int)d)); }
        //}

    }
}
