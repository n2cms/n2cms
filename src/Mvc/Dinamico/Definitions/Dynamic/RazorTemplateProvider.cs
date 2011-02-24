using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Definitions;
using N2.Engine;
using System.IO;
using N2.Edit.FileSystem;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Web.Mvc.Html;
using Dinamico.Models;
using N2.Edit;
using N2.Persistence;
using N2.Web.Mvc;
using N2;
using System.Diagnostics;
using N2.Definitions.Runtime;
using System.Web.Hosting;
using System.Web.Caching;

namespace Dinamico.Definitions.Dynamic
{
	[Service(typeof(IProvider<HttpContextBase>))]
	public class HttpContextProvider : IProvider<HttpContextBase>
	{
		#region IProvider<HttpContextBase> Members

		public HttpContextBase Get()
		{
			return new HttpContextWrapper(HttpContext.Current);
		}

		public IEnumerable<HttpContextBase> GetAll()
		{
			return new[] { Get() };
		}

		#endregion
	}

	[Service(typeof(IProvider<ViewEngineCollection>))]
	public class ViewEngineCollectionProvider : IProvider<ViewEngineCollection>
	{
		#region IProvider<ViewEngineCollection> Members

		public ViewEngineCollection Get()
		{
			return ViewEngines.Engines;
		}

		public IEnumerable<ViewEngineCollection> GetAll()
		{
			return new[] { ViewEngines.Engines };
		}

		#endregion
	}

	[Service(typeof(IProvider<VirtualPathProvider>))]
	public class VirtualPathProviderProvider : IProvider<VirtualPathProvider>
	{
		#region IProvider<VirtualPathProvider> Members

		public VirtualPathProvider Get()
		{
			return HostingEnvironment.VirtualPathProvider;
		}

		public IEnumerable<VirtualPathProvider> GetAll()
		{
			return new[] { HostingEnvironment.VirtualPathProvider };
		}

		#endregion
	}

	[Service]
	public class RazorTemplateRegistrator
	{
		public RazorTemplateRegistrator()
		{
			QueuedRegistrations = new Queue<Type>();
		}

		public RazorTemplateRegistrator Add<T>() where T : Controller
		{
			QueuedRegistrations.Enqueue(typeof(T));
			if (RegistrationAdded != null)
				RegistrationAdded.Invoke(this, new EventArgs());

			return this;
		}

		public event EventHandler RegistrationAdded;

		public Queue<Type> QueuedRegistrations { get; set; }
	}

	[Service(typeof(ITemplateProvider))]
	public class RazorTemplateProvider : ITemplateProvider
	{
		public static bool SwallowExceptions { get; set; }

		IProvider<HttpContextBase> httpContextProvider;
		IProvider<ViewEngineCollection> viewEnginesProvider;
		IProvider<VirtualPathProvider> vppProvider;
		ContentActivator activator;
		RazorTemplateRegistrator registrator;
		List<Source> sources = new List<Source>();
		bool rebuild = true;

		public RazorTemplateProvider(RazorTemplateRegistrator registrator, ContentActivator activator, IProvider<HttpContextBase> httpContextProvider, IProvider<ViewEngineCollection> viewEnginesProvider, IProvider<VirtualPathProvider> vppProvider)
		{
			this.registrator = registrator;
			registrator.RegistrationAdded += registrator_RegistrationAdded;

			//this.fs = fs;
			this.activator = activator;
			this.httpContextProvider = httpContextProvider;
			this.viewEnginesProvider = viewEnginesProvider;
			this.vppProvider = vppProvider;
			
			HandleRegistrationQueue();
		}

		void registrator_RegistrationAdded(object sender, EventArgs e)
		{
			HandleRegistrationQueue();
		}

		private void HandleRegistrationQueue()
		{
			while (registrator.QueuedRegistrations.Count > 0)
			{
				var controllerType = registrator.QueuedRegistrations.Dequeue();
				string controllerName = controllerType.Name.Substring(0, controllerType.Name.Length - "Controller".Length);
				Type modelType = typeof(ContentItem);
				Type contentControllerType = Utility.GetBaseTypes(controllerType).FirstOrDefault(t => t.GetGenericTypeDefinition() == typeof(ContentController<>));
				if (contentControllerType != null)
					modelType = contentControllerType.GetGenericArguments().First();
				sources.Add(new Source { ControllerName = controllerName, ModelType = modelType });
				rebuild = true;
			}
		}

		#region ITemplateProvider Members

		public IEnumerable<TemplateDefinition> GetTemplates(Type contentType)
		{
			var httpContext = httpContextProvider.Get();
			try
			{
				httpContext.Request.GetType();
			}
			catch (Exception)
			{
				return new TemplateDefinition[0];
			}
			
			const string cacheKey = "RazorDefinitions";
			var definitions = httpContext.Cache[cacheKey] as IEnumerable<ItemDefinition>;
			if (definitions == null || rebuild)
			{
				var vpp = vppProvider.Get();
				var pairs = BuildDefinitions(vpp, httpContext).ToList();
				definitions = pairs.Select(p => p.Definition).ToList();

				var files = pairs.SelectMany(p => p.VirtualPaths).Distinct().ToList();
				var dirs = files.Select(f => f.Substring(0, f.LastIndexOf('/'))).Distinct();
				var cacheDependency = vpp.GetCacheDependency(dirs.FirstOrDefault(), dirs, DateTime.UtcNow);

				httpContext.Cache.Remove(cacheKey);
				httpContext.Cache.Add(cacheKey, definitions, cacheDependency, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, new CacheItemRemovedCallback(delegate { Debug.WriteLine("Razor template changed"); }));
				rebuild = false;
			}
			var templates = definitions.Where(d => d.ItemType == contentType).Select(d =>
				{
					var td = new TemplateDefinition();
					td.Definition = d;
					td.Description = d.Description;
					td.Name = d.Template;
					td.Original = null;
					td.Template = activator.CreateInstance(d.ItemType, null);
					td.Template["TemplateName"] = d.Template;
					td.TemplateUrl = null;
					td.Title = d.Title;
					td.ReplaceDefault = "Index".Equals(d.Template, StringComparison.InvariantCultureIgnoreCase);
					return td;
				}).ToArray();

			if (templates.Length > 1)
			{
				foreach (var t in templates)
				{
					t.Definition.Add(new TemplateSelectorAttribute { Name = "TemplateName", Title = "Template", AllTemplates = templates, ContainerName = "Advanced", HelpTitle = "The page must be saved for another template's fields to appear" });
				}
			}

			return templates;
		}

		private IEnumerable<RazorFileDefinition> BuildDefinitions(VirtualPathProvider vpp, HttpContextBase httpContext)
		{
			foreach (var source in sources)
			{
				string virtualDir = "~/Views/" + source.ControllerName;
				if (!vpp.DirectoryExists(virtualDir))
					continue;

				foreach (var file in vpp.GetDirectory(virtualDir).Files.OfType<VirtualFile>().Where(f => f.Name.EndsWith(".cshtml")))
				{
					var definition = GetDefinition(httpContext, file, source.ControllerName, source.ModelType);
					if (definition != null)
						yield return definition;
				}
			}
		}

		private RazorFileDefinition GetDefinition(HttpContextBase httpContext, VirtualFile file, string controllerName, Type modelType)
		{
			var cctx = new ControllerContext(httpContext, new RouteData(), new StubController());
			cctx.RouteData.Values.Add("controller", controllerName);
			var result = viewEnginesProvider.Get().FindView(cctx, file.VirtualPath, null);

			if (result.View == null)
				return null;

			if (!SwallowExceptions)
				return RenderView(file, modelType, cctx, result);

			try
			{
				return RenderView(file, modelType, cctx, result);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex);
				return null;
			}
		}

		private static RazorFileDefinition RenderView(VirtualFile file, Type modelType, ControllerContext cctx, ViewEngineResult result)
		{
			var re = new ContentRegistration();
			re.Template = N2.Web.Url.RemoveExtension(file.Name);
			re.IsDefined = false;
			using (StringWriter sw = new StringWriter())
			{
				var vdd = new ViewDataDictionary { Model = modelType != null ? Activator.CreateInstance(modelType) : new StubItem() };
				cctx.Controller.ViewData = vdd;
				vdd["RegistrationExpression"] = re;
				result.View.Render(new ViewContext(cctx, result.View, vdd, new TempDataDictionary(), sw), sw);

				if (re.IsDefined)
					return new RazorFileDefinition
					{
						Definition = re.CreateDefinition(N2.Definitions.Static.DefinitionTable.Instance),
						VirtualPaths = new[] { file.VirtualPath }.Union(re.TouchedPaths)
					};
				return null;
			}
		}

		public TemplateDefinition GetTemplate(N2.ContentItem item)
		{
			string templateName = item["TemplateName"] as string;
			if (templateName == null)
				return null;

			return GetTemplates(item.GetContentType()).Where(t => t.Name == templateName).Select(t =>
				{
					t.Original = t.Template;
					t.Template = item;
					return t;
				}).FirstOrDefault();
		}

		#endregion



		class RazorFileDefinition
		{
			public IEnumerable<string> VirtualPaths { get; set; }
			public ItemDefinition Definition { get; set; }
		}

		class Source
		{
			public string ControllerName { get; set; }
			public Type ModelType { get; set; }
		}

		class StubController : Controller
		{
		}

		class StubItem : ContentItem
		{
		}
	}
}