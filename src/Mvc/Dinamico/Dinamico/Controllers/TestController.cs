using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Dinamico.Models;
using N2;
using N2.Definitions.Runtime;
using N2.Definitions.Static;
using N2.Edit.FileSystem;
using N2.Web;
using N2.Web.Mvc;
using N2.Web.Mvc.Html;

namespace Dinamico.Controllers
{
	public class TestController : Controller
	{
		//
		// GET: /Test/

		public ActionResult Index()
		{
			var sw = new StringWriter();
			var files = Context.Current.Resolve<IFileSystem>();
			var expressions = new List<ContentRegistration>();
			foreach (
				var file in files.GetFiles("~/Dinamico/Themes/Default/Views/ContentPages/").Where(f => f.Name.EndsWith(".cshtml")))
			{
				var cctx = new ControllerContext(ControllerContext.HttpContext, new RouteData(), new ContentPagesController());
				cctx.RouteData.Values.Add("controller", "DynamicPages");
				var v = ViewEngines.Engines.FindView(cctx, file.VirtualPath, null);

				if (v.View == null)
					sw.Write(string.Join(", ", v.SearchedLocations.ToArray()));
				else
				{
					var temp = new ContentPage();
					cctx.RequestContext.RouteData.ApplyCurrentPath(new PathData(temp));
					var vdd = new ViewDataDictionary {Model = temp};
					var re = new ContentRegistration(new DefinitionMap().GetOrCreateDefinition(typeof(ContentPage)).Clone());
					RegistrationExtensions.SetRegistrationExpression(cctx.HttpContext, re);
					v.View.Render(new ViewContext(cctx, v.View, vdd, new TempDataDictionary(), sw), sw);
					expressions.Add(re);
				}
			}
			return View(expressions);
		}
	}
}