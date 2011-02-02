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
using N2.Definitions.Dynamic;

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
		IProvider<HttpContextBase> httpContextProvider;
		IProvider<ViewEngineCollection> viewEnginesProvider;
		IFileSystem fs;
		ContentActivator activator;
		RazorTemplateRegistrator registrator;
		List<Tuple<string, Type>> sources = new List<Tuple<string, Type>>();
		bool rebuild = true;

		public RazorTemplateProvider(RazorTemplateRegistrator registrator, IFileSystem fs, ContentActivator activator, IProvider<HttpContextBase> httpContextProvider, IProvider<ViewEngineCollection> viewEnginesProvider)
		{
			this.registrator = registrator;
			registrator.RegistrationAdded += registrator_RegistrationAdded;

			this.fs = fs;
			this.activator = activator;
			this.httpContextProvider = httpContextProvider;
			this.viewEnginesProvider = viewEnginesProvider;
			
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
				sources.Add(new Tuple<string, Type>(controllerName, modelType));
				rebuild = true;
			}
		}

		#region ITemplateProvider Members

		class StubController : Controller
		{
		}

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

			var definitions = httpContext.Cache["RazorDefinitions"] as IEnumerable<ItemDefinition>;
			if (definitions == null || rebuild)
			{
				httpContext.Cache["RazorDefinitions"] = definitions = BuildDefinitions(httpContext).ToList();
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

		public IEnumerable<ItemDefinition> BuildDefinitions(HttpContextBase httpContext)
		{
			foreach (var source in sources)
			{
				string controllerName = source.Item1;
				Type modelType = source.Item2;

				foreach (var file in fs.GetFiles("~/Views/" + controllerName).Where(f => f.Name.EndsWith(".cshtml")))
				{
					var definition = GetDefinition(httpContext, file, controllerName, modelType);
					if (definition != null)
						yield return definition;
				}
			}
		}

		private ItemDefinition GetDefinition(HttpContextBase httpContext, FileData file, string controllerName, Type modelType)
		{
			var cctx = new ControllerContext(httpContext, new RouteData(), new StubController());
			cctx.RouteData.Values.Add("controller", controllerName);
			var v = viewEnginesProvider.Get().FindView(cctx, file.VirtualPath, null);

			if (v.View == null)
				return null;

			var re = new DefinitionRegistrationExpression();
			re.Template = N2.Web.Url.RemoveExtension(file.Name);
			re.IsDefined = false;

			try
			{
				using (StringWriter sw = new StringWriter())
				{
					var vdd = new ViewDataDictionary { Model = new DynamicPage() };
					vdd["RegistrationExpression"] = re;
					v.View.Render(new ViewContext(cctx, v.View, vdd, new TempDataDictionary(), sw), sw);

					if (re.IsDefined)
						return re.CreateDefinition(N2.Definitions.Static.DefinitionDictionary.Instance);
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex);
			}

			return null;
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
	}
}