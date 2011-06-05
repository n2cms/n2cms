using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using N2.Edit.FileSystem;
using System.Web.Routing;
using N2.Web.Mvc.Html;
using Dinamico.Models;
using N2.Definitions.Runtime;

namespace Dinamico.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/

        public ActionResult Index()
		{
			StringWriter sw = new StringWriter();
			IFileSystem files = N2.Context.Current.Resolve<IFileSystem>();
			List<ContentRegistration> expressions = new List<ContentRegistration>();
			foreach (var file in files.GetFiles("~/Views/DynamicPages/").Where(f => f.Name.EndsWith(".cshtml")))
			{
				var cctx = new ControllerContext(ControllerContext.HttpContext, new RouteData(), new ContentPagesController());
				cctx.RouteData.Values.Add("controller", "DynamicPages");
				var v = ViewEngines.Engines.FindView(cctx, file.VirtualPath, null);

				if (v.View == null)
					sw.Write(string.Join(", ", v.SearchedLocations.ToArray()));
				else
				{
					var vdd = new ViewDataDictionary { Model = new ContentPage() };
					var re = new ContentRegistration();
					vdd["RegistrationExpression"] = re;
					v.View.Render(new ViewContext(cctx, v.View, vdd, new TempDataDictionary(), sw), sw);
					expressions.Add(re);
				}
			}
			return View(expressions);
		}

    }
}
