using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Definitions.Static;
using N2.Engine;
using N2.Web.Mvc;
using N2.Web;

namespace N2.Definitions.Runtime
{
	[Service]
	public class ViewTemplateAnalyzer
	{
		private readonly Logger<ViewTemplateAnalyzer> logger;
		IProvider<ViewEngineCollection> viewEnginesProvider;
		DefinitionMap map;
		DefinitionBuilder builder;

		public ViewTemplateAnalyzer(IProvider<ViewEngineCollection> viewEnginesProvider, DefinitionMap map, DefinitionBuilder builder)
		{
			this.viewEnginesProvider = viewEnginesProvider;
			this.map = map;
			this.builder = builder;
		}

		public virtual IEnumerable<ContentRegistration> AnalyzeViews(VirtualPathProvider vpp, HttpContextBase httpContext, IEnumerable<ViewTemplateSource> sources)
		{
			var registrations = new List<ContentRegistration>();
			foreach (var source in sources)
			{
				string virtualDir = Url.ResolveTokens(Url.ThemesUrlToken) + "Default/Views/" + source.ControllerName;

				logger.DebugFormat("Analyzing directory {0}", virtualDir);

				if (!vpp.DirectoryExists(virtualDir))
				{
					virtualDir = "~/Views/" + source.ControllerName;
					if (!vpp.DirectoryExists(virtualDir))
						continue;
				}

				foreach (var file in vpp.GetDirectory(virtualDir).Files.OfType<VirtualFile>().Where(f => f.Name.EndsWith(source.ViewFileExtension)))
				{
					logger.DebugFormat("Analyzing file {0}", file.VirtualPath);

					var registration = AnalyzeView(httpContext, file, source.ControllerName, source.ModelType);
					if (registration != null)
						registrations.Add(registration);
				}
			}
			return registrations;
		}

		private ContentRegistration AnalyzeView(HttpContextBase httpContext, VirtualFile file, string controllerName, Type modelType)
		{
			if (modelType == null || !typeof(ContentItem).IsAssignableFrom(modelType) || modelType.IsAbstract)
				return null;

			var model = Activator.CreateInstance(modelType) as ContentItem;

			var rd = new RouteData();
			bool isPage = model.IsPage;
			RouteExtensions.ApplyCurrentPath(rd, controllerName, Path.GetFileNameWithoutExtension(file.Name), new PathData(isPage ? model : new StubItem(), isPage ? null : model));

			var cctx = new ControllerContext(httpContext, rd, new StubController());

			var result = isPage
				? viewEnginesProvider.Get().FindView(cctx, file.VirtualPath, null)
				: viewEnginesProvider.Get().FindPartialView(cctx, file.VirtualPath);


			if (result.View == null)
				return null;

			return RenderViewForRegistration(file, modelType, cctx, result);
		}

		private ContentRegistration RenderViewForRegistration(VirtualFile file, Type modelType, ControllerContext cctx, ViewEngineResult result)
		{
			var re = new ContentRegistration(map.CreateDefinition(modelType, N2.Web.Url.RemoveAnyExtension(file.Name)));
			re.IsDefined = false;
			re.Context.TouchedPaths.Add(file.VirtualPath);

			using (StringWriter sw = new StringWriter())
			{
				var vdd = new ViewDataDictionary();
				cctx.Controller.ViewData = vdd;
				N2.Web.Mvc.Html.RegistrationExtensions.SetRegistrationExpression(cctx.HttpContext, re);

				try
				{
					logger.DebugFormat("Rendering view {0} for registrations", file.VirtualPath);
					result.View.Render(new ViewContext(cctx, result.View, vdd, new TempDataDictionary(), sw), sw);
					logger.DebugFormat("Rendered view {0}, editables = {1}, defined = {2}", file.VirtualPath, re.Definition.Editables.Count, re.IsDefined);
				}
				catch (Exception ex)
				{
					logger.Error(file.VirtualPath, ex);
					if (re.IsDefined)
						throw;
					return null;
				}
				finally
				{
					N2.Web.Mvc.Html.RegistrationExtensions.SetRegistrationExpression(cctx.HttpContext, null);
				}

				if (re.IsDefined)
				{
					return re;
				}
				return null;
			}
		}

		class StubController : Controller
		{
		}
		class StubItem : ContentItem
		{
		}
	}
}