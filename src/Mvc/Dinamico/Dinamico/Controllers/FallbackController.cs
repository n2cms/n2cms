using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Web;
using Dinamico.Models;
using N2.Web.Mvc;

namespace Dinamico.Controllers
{
    [Controls(typeof(PageModelBase))]
    public class FallbackController : ContentController
    {
    }
}
