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


	[Service(typeof(ITemplateProvider))]
	public class RazorTemplateProvider : ITemplateProvider
	{
		IProvider<HttpContextBase> httpContextProvider;
		IFileSystem fs;
		ContentActivator activator;

		public RazorTemplateProvider(IProvider<HttpContextBase> httpContextProvider, IFileSystem fs, ContentActivator activator)
		{
			this.httpContextProvider = httpContextProvider;
			this.fs = fs;
			this.activator = activator;
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

			var definitions = httpContext.Items["RazorDefinitions"] as IEnumerable<ItemDefinition>;
			if (definitions == null)
			{
				httpContext.Items["RazorDefinitions"] = definitions = BuildDefinitions(httpContext).ToList();
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
					t.Definition.Add(new TemplateSelectorAttribute { Name = "TemplateName", AllTemplates = templates });
				}
			}

			return templates;
		}

		public IEnumerable<ItemDefinition> BuildDefinitions(HttpContextBase httpContext)
		{
			StringWriter sw = new StringWriter();
			foreach (var file in fs.GetFiles("~/Views/DynamicPages/").Where(f => f.Name.EndsWith(".cshtml")))
			{
				var cctx = new ControllerContext(httpContext, new RouteData(), new StubController());
				cctx.RouteData.Values.Add("controller", "DynamicPages");
				var v = ViewEngines.Engines.FindView(cctx, file.VirtualPath, null);

				if (v.View == null)
					continue;

				var re = new DefinitionRegistrationExpression();
				re.Template = N2.Web.Url.RemoveExtension(file.Name);
				re.IsDefined = false;

				httpContext.Items["RegistrationExpression"] = re;
				try
				{
					v.View.Render(new ViewContext(cctx, v.View, new ViewDataDictionary { Model = new DynamicPage() }, new TempDataDictionary(), sw), sw);
				}
				catch (Exception)
				{
					continue;
				}
				finally
				{
					httpContext.Items["RegistrationExpression"] = null;
				}

				if (!re.IsDefined)
					continue;

				var id = new ItemDefinition(re.ItemType);
				id.Initialize(re.ItemType);

				foreach (IDefinitionRefiner refiner in re.ItemType.GetCustomAttributes(typeof(IDefinitionRefiner), true))
					refiner.Refine(id, new[] { id });

				id.Title = re.Title;
				id.Template = re.Template;

				foreach (var c in re.Containables)
				{
					id.Add(c);
				}
				foreach (var e in re.Editables)
					id.Add(e);

				yield return id;
			}
		}

		public TemplateDefinition GetTemplate(N2.ContentItem item)
		{
			string templateName = item["TemplateName"] as string;
			if (templateName == null)
				return null;

			return GetTemplates(item.GetContentType()).Where(t => t.Definition.Template == templateName).Select(t =>
				{
					t.Original = t.Template;
					t.Template = item;
					return t;
				}).FirstOrDefault();
		}

		#endregion
	}
}