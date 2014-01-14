using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Web;
using N2.Templates.Mvc.Models.Parts;
using N2.Templates.Mvc.Models;

namespace N2.Templates.Mvc.Controllers
{
    [Controls(typeof(ABTestingContainer))]
    public class ABTestingController : N2.Web.Mvc.ContentController<ABTestingContainer>
    {
        //
        // GET: /ABTesting/

        public override ActionResult Index()
        {
            var percentages = new [] { CurrentItem.Zone1Percentage, CurrentItem.Zone2Percentage, CurrentItem.Zone3Percentage };
            int sum = percentages.Sum();
            int random = new Random().Next(sum);
            for (int i = 0; i < percentages.Length; i++)
            {
                random -= percentages[i];
                if(random < 0)
                {
                    var model = new ABTestingModel { CurrentItem = CurrentItem, ChosenZone = "Zone" + (i + 1), Zones = new []{ "Zone1", "Zone2", "Zone3" }, Percentages = percentages, PercentageSum = sum };
                    return View(model);
                }
            }

            return View("Empty");
        }
    }

}
