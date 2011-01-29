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
			foreach (var file in files.GetFiles("~/Views/DynamicPages/").Where(f => f.Name.EndsWith(".cshtml")))
			{
				var cctx = new ControllerContext(ControllerContext.HttpContext, new RouteData(), new DynamicPagesController());
				cctx.RouteData.Values.Add("controller", "DynamicPages");
				var v = ViewEngines.Engines.FindView(cctx, file.VirtualPath, null);

				var re = new DefinitionRegistrationExpression();
				ControllerContext.HttpContext.Items["RegistrationExpression"] = re;
				if (v.View == null)
					sw.Write(string.Join(", ", v.SearchedLocations.ToArray()));
				else
					v.View.Render(new ViewContext(cctx, v.View, new ViewDataDictionary { Model = new DynamicPage() }, new TempDataDictionary(), sw), sw);
				ControllerContext.HttpContext.Items["RegistrationExpression"] = null;
				return View(re);
			}
			return Content(sw.ToString());
        }

    }
}
