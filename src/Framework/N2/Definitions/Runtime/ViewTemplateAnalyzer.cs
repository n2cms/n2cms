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
		// ReSharper disable FieldCanBeMadeReadOnly.Local
		private Logger<ViewTemplateAnalyzer> logger;
		IProvider<ViewEngineCollection> viewEnginesProvider;
		DefinitionMap map;
		DefinitionBuilder builder;
		// ReSharper restore FieldCanBeMadeReadOnly.Local

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
				var virtualDirList = new List<string>
				{
					Url.ResolveTokens(Url.ThemesUrlToken) + "Default/Views/" + source.ControllerName,
					"~/Views/" + source.ControllerName
				};

				virtualDirList.AddRange(
					from virtualDirectory in vpp.GetDirectory("~/Areas/").Directories.Cast<VirtualDirectory>()
					let virtualPath = String.Format("~/Areas/{0}/Views/{1}", virtualDirectory.Name, source.ControllerName)
					select virtualPath);

				foreach (var virtualDir in virtualDirList.Where(vpp.DirectoryExists))
				{
					logger.Debug(String.Format("Analyzing directory {0}", virtualDir));
					foreach (var file in vpp.GetDirectory(virtualDir).Files.OfType<VirtualFile>())
					{
						Debug.Assert(file.Name != null, "file.Name != null");
						if (!file.Name.EndsWith(source.ViewFileExtension) || file.Name.StartsWith("_"))
						{
							logger.Info(String.Format("Skipping file {0}", file.VirtualPath));
							continue;
						}
						logger.Debug(String.Format("Analyzing file {0}", file.VirtualPath));

						if (httpContext.IsDebuggingEnabled)
						{
							AnalyzeView(httpContext, file, source.ControllerName, source.ModelType, registrations);
						}
						else
						{
							try
							{
								AnalyzeView(httpContext, file, source.ControllerName, source.ModelType, registrations);
							}
							catch (Exception ex)
							{
								logger.Error(ex);
							}
						}
					}
				}
			}
			return registrations;
		}

		private ContentRegistration AnalyzeView(HttpContextBase httpContext, VirtualFile file, string controllerName, Type modelType, List<ContentRegistration> registrations)
		{
			if (modelType == null || !typeof(ContentItem).IsAssignableFrom(modelType) || modelType.IsAbstract)
				return null;

			var model = Activator.CreateInstance(modelType) as ContentItem;

			var rd = new RouteData();
			var isPage = model != null && model.IsPage;
			RouteExtensions.ApplyCurrentPath(rd, controllerName, Path.GetFileNameWithoutExtension(file.Name), new PathData(isPage ? model : new StubItem(), isPage ? null : model));

			var cctx = new ControllerContext(httpContext, rd, new StubController());

			var result = isPage
				? viewEnginesProvider.Get().FindView(cctx, file.VirtualPath, null)
				: viewEnginesProvider.Get().FindPartialView(cctx, file.VirtualPath);


			return result.View == null ? null : RenderViewForRegistration(file, modelType, cctx, result, registrations);
		}

		// ReSharper disable RedundantNameQualifier
		private ContentRegistration RenderViewForRegistration(VirtualFileBase file, Type modelType, ControllerContext cctx, ViewEngineResult result, List<ContentRegistration> registrations)
		{
			if (file == null)
				throw new ArgumentNullException("file");

			// ReSharper disable once UseObjectOrCollectionInitializer
			var fileName = N2.Web.Url.RemoveAnyExtension(file.Name);
            var re = registrations.FirstOrDefault(cr => cr.Definition.ItemType == modelType && cr.Definition.TemplateKey == fileName)
				?? new ContentRegistration(map.CreateDefinition(modelType, fileName)) { IsDefined = false };
			re.Context.TouchedPaths.Add(file.VirtualPath);

			using (var sw = new StringWriter())
			{
				var vdd = new ViewDataDictionary();
				cctx.Controller.ViewData = vdd;
				N2.Web.Mvc.Html.RegistrationExtensions.SetRegistrationExpression(cctx.HttpContext, re);

				try
				{
					logger.DebugFormat("Rendering view {0} for registrations", file.VirtualPath);
					result.View.Render(new ViewContext(cctx, result.View, vdd, new TempDataDictionary(), sw), sw);
					logger.DebugFormat("Rendered view {0}, editables = {1}, defined = {2}", file.VirtualPath, re.Definition.Editables.Count, re.IsDefined);

					if (!registrations.Contains(re))
						registrations.Add(re);
				}
				catch (Exception ex)
				{
					logger.Error(file.VirtualPath, ex);
					if (re.IsDefined)
						throw new Exception(String.Format("Failed to render view {0} for registrations", file.VirtualPath), ex);
					return null;
				}
				finally
				{
					N2.Web.Mvc.Html.RegistrationExtensions.SetRegistrationExpression(cctx.HttpContext, null);
				}

				return re.IsDefined ? re : null;
			}
		}
		// ReSharper restore RedundantNameQualifier

		class StubController : Controller
		{
		}
		class StubItem : ContentItem
		{
		}
	}
}
