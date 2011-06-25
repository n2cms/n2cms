﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using N2.Definitions.Static;
using N2.Engine;
using N2.Persistence;

namespace N2.Definitions.Runtime
{
	[Service(typeof(ITemplateProvider))]
	public class ViewTemplateProvider : ITemplateProvider
	{
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
				return new TemplateDefinition[0];

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
			lock (this)
			{
				if (definitions == null || rebuild)
				{
					DequeueRegistrations();

					var vpp = vppProvider.Get();
					var descriptions = analyzer.AnalyzeViews(vpp, httpContext, sources).ToList();
					definitions = BuildDefinitions(descriptions);

					var files = descriptions.SelectMany(p => p.TouchedPaths).Distinct().ToList();
					//var dirs = files.Select(f => f.Substring(0, f.LastIndexOf('/'))).Distinct();
					var cacheDependency = vpp.GetCacheDependency(files.FirstOrDefault(), files, DateTime.UtcNow);

					httpContext.Cache.Remove(cacheKey);
					httpContext.Cache.Add(cacheKey, definitions, cacheDependency, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, new CacheItemRemovedCallback(delegate { Debug.WriteLine("Razor template changed"); }));
					rebuild = false;
				}
			}

			var templates = definitions.Where(d => d.ItemType == contentType).Select(d =>
				{
					var td = new TemplateDefinition();
					td.Definition = d;
					td.Description = d.Description;
					td.Name = d.TemplateKey;
					td.Original = () => null;
					td.Template = () => activator.CreateInstance(d.ItemType, null, d.TemplateKey);
					td.TemplateUrl = null;
					td.Title = d.Title;
					td.ReplaceDefault = "Index".Equals(d.TemplateKey, StringComparison.InvariantCultureIgnoreCase);
					return td;
				}).ToArray();

			return templates;
		}

		public TemplateDefinition GetTemplate(N2.ContentItem item)
		{
			var httpContext = httpContextProvider.Get();
			if (httpContext != null)
				if (N2.Web.Mvc.Html.RegistrationExtensions.GetRegistrationExpression(httpContext) != null)
					return null;

			string templateKey = item.TemplateKey;
			if (templateKey == null)
				return null;

			return GetTemplates(item.GetContentType()).Where(t => t.Name == templateKey).Select(t =>
				{
					t.Original = t.Template;
					t.Template = () => item;
					return t;
				}).FirstOrDefault();
		}

		private IEnumerable<ItemDefinition> BuildDefinitions(List<ViewTemplateDescription> registrations)
		{
			var definitions = registrations.Select(r => r.Definition).ToList();
			builder.ExecuteRefiners(definitions);
			foreach (var registration in registrations)
				registration.Registration.AppendToDefinition(registration.Definition);
			return definitions;
		}

		#endregion
	}
}