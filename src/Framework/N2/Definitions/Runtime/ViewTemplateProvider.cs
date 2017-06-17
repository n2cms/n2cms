using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using N2.Definitions.Static;
using N2.Engine;
using N2.Persistence;
using N2.Web.Mvc.Html;

namespace N2.Definitions.Runtime
{
	[Service(typeof(ITemplateProvider))]
	public class ViewTemplateProvider : ITemplateProvider
	{
		private readonly Logger<ViewTemplateProvider> logger;
		IProvider<HttpContextBase> httpContextProvider;
		IProvider<VirtualPathProvider> vppProvider;
		ContentActivator activator;
		DefinitionBuilder builder;
		ViewTemplateRegistrator registrator;
		ViewTemplateAnalyzer analyzer;
		List<ViewTemplateSource> sources = new List<ViewTemplateSource>();
		bool rebuild = true;

		public ViewTemplateProvider(ViewTemplateRegistrator registrator, ViewTemplateAnalyzer analyzer, ContentActivator activator, DefinitionBuilder builder, IProvider<HttpContextBase> httpContextProvider, IProvider<VirtualPathProvider> vppProvider)
		{
			SortOrder = -1000;

			this.registrator = registrator;
			this.analyzer = analyzer;
			this.activator = activator;
			this.builder = builder;
			this.httpContextProvider = httpContextProvider;
			this.vppProvider = vppProvider;

			registrator.RegistrationAdded += (s, a) => rebuild = true;
		}

		private void DequeueRegistrations()
		{

			while (registrator.QueuedRegistrations.Count > 0)
			{
				var source = registrator.QueuedRegistrations.Dequeue();
				sources.Add(source);
			}
		}

		#region ITemplateProvider Members

		public IEnumerable<TemplateDefinition> GetTemplates(Type contentType)
		{
			var httpContext = httpContextProvider.Get();
			if (httpContext == null)
			{
				logger.Warn("Trying to get templates with no context");
				return Enumerable.Empty<TemplateDefinition>();
			}

			try
			{
				httpContext.Request.GetType();
			}
			catch (Exception ex)
			{
				logger.Warn("Trying to get templates with invalid context", ex);
				return Enumerable.Empty<TemplateDefinition>();
			}
			
			const string cacheKey = "RazorDefinitions";
			var definitions = httpContext.Cache[cacheKey] as IEnumerable<ItemDefinition>;

			if (definitions == null || rebuild)
			{
				lock (this)
				{
					if (definitions == null || rebuild)
					{
						logger.DebugFormat("Dequeuing {0} registrations", registrator.QueuedRegistrations.Count);

						DequeueRegistrations();

						var vpp = vppProvider.Get();
						var descriptions = analyzer.AnalyzeViews(vpp, httpContext, sources).ToList();
						logger.DebugFormat("Got {0} descriptions", descriptions.Count);
						definitions = BuildDefinitions(descriptions).ToList();
						logger.Debug("Built definitions");

						var files = descriptions.SelectMany(p => p.Context.TouchedPaths).Distinct().ToList();
						logger.DebugFormat("Setting up cache dependency on {0} files", files.Count);
						var cacheDependency = vpp.GetCacheDependency(files.FirstOrDefault(), files, DateTime.UtcNow);

						httpContext.Cache.Remove(cacheKey);
						httpContext.Cache.Add(cacheKey, definitions, cacheDependency, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, new CacheItemRemovedCallback(delegate
						{
							logger.Debug("Razor template changed");
						}));
						rebuild = false;
					}
				}
			}

			var templates = definitions.Where(d => d.ItemType == contentType).Select(d =>
				{
					var td = new TemplateDefinition();
					td.Definition = d;
					td.Description = d.Description;
					td.Name = d.TemplateKey;
					td.OriginalFactory = () => null;
					td.TemplateFactory = () => activator.CreateInstance(d.ItemType, null, d.TemplateKey);
					td.TemplateUrl = null;
					td.Title = d.Title;
					td.ReplaceDefault = "Index".Equals(d.TemplateKey, StringComparison.InvariantCultureIgnoreCase);
					return td;
				}).ToArray();

			return templates;
		}

		public TemplateDefinition GetTemplate(ContentItem item)
		{
			var httpContext = httpContextProvider.Get();
			if (httpContext != null)
				if (RegistrationExtensions.GetRegistrationExpression(httpContext) != null)
					return null;

			string templateKey = item.TemplateKey;
			if (templateKey == null)
				return null;

			return GetTemplates(item.GetContentType()).Where(t => t.Name == templateKey).Select(t =>
				{
					t.OriginalFactory = t.TemplateFactory;
					t.TemplateFactory = () => item;
					return t;
				}).FirstOrDefault();
		}

		private IEnumerable<ItemDefinition> BuildDefinitions(List<ContentRegistration> registrations)
		{
			var definitions = registrations.Select(r => r.Definition).ToList();
			builder.ExecuteRefiners(definitions);
			return registrations.Select(r => r.Finalize()).ToList();
		}

		#endregion

		/// <summary>The order this template provider should be invoked, default 0.</summary>
		public int SortOrder { get; set; }
	}
}
