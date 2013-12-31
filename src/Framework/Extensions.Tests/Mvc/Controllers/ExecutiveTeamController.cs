using System;
using System.Web.Mvc;
using N2.Extensions.Tests.Mvc.Models;
using N2.Web;
using N2.Web.Mvc;

namespace N2.Extensions.Tests.Mvc.Controllers
{
    [Controls(typeof(ExecutiveTeamPage))]
    public class ExecutiveTeamController : ContentController<ExecutiveTeamPage>
    {
        public override ActionResult Index()
        {
            throw new NotImplementedException();
        }
    }
}
