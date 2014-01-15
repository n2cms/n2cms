using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Templates.Mvc.Models.Parts;

namespace N2.Templates.Mvc.Models
{
    public class ABTestingModel : ContentModelBase<ABTestingContainer>
    {
        public string ChosenZone { get; set; }
        public string[] Zones { get; set; }
        public int[] Percentages { get; set; }
        public int PercentageSum { get; set; }
    }
}
