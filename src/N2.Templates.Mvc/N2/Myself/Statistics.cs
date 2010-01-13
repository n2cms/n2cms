using System;
using System.Collections.Generic;
using System.Text;
using N2.Integrity;
using N2.Details;

namespace N2.Management.Myself
{
    [PartDefinition("Statistics", Name = "Statistics", TemplateUrl = "~/N2/Myself/Statistics.ascx")]
    [WithEditableTitle("Title", 10)]
    [RestrictParents(typeof(RootPage))]
    [AllowedZones("Left", "Center", "Right")]
    public class StatisticsPart : ContentItem
    {
        [Editable("Latest changes max count", typeof(System.Web.UI.WebControls.TextBox), "Text", 100)]
        public virtual int LatestChangesMaxCount
        {
            get { return (int)(GetDetail("LatestChangesMaxCount") ?? 5); }
            set { SetDetail("LatestChangesMaxCount", value); }
        }
    }
}