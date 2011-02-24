using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Web.Mvc;
using Dinamico.Models;
using N2.Web;

namespace Dinamico.Controllers
{
	[Controls(typeof(ListingPage))]
    public class ListingPagesController : ContentController<ListingPage>
    {
    }
}
