using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Reflection;
using System.Diagnostics;

using Castle.Core;

using N2.Engine;
using N2.Persistence;
using N2.Web;
using N2.Definitions;
using N2.Integrity;
using N2.Security;
using N2.Edit;
using N2.Persistence.NH;
using N2.Persistence.NH.Finder;
using N2.Serialization;
using N2.MediumTrust.Configuration;
using N2.Details;
using N2.Persistence.Finder;
using N2.Plugin;
using N2.Parts;
using N2.Configuration;
using N2.Globalization;
using System.Web.Configuration;
using N2.Installation;
using System.Configuration;
using N2.Web.UI;

namespace N2.MediumTrust.Engine
{
	public class MediumTrustEngine : IEngine
	{
        ISecurityManager securityManager;
        IDefinitionManager definitions;
        IUrlParser urlParser;
        IEditManager editManager;
        IUrlRewriter rewriter;
        IIntegrityManager integrityManager;
        IHost host;
        IRequestLifeCycleHandler lifeCycleHandler;
        IPersister persister;

        IDictionary<Type, object> resolves = new Dictionary<Type, object>();
            
		public MediumTrustEngine()
		{
            IWebContext webContext;
            HostSection hostConfiguration = (HostSection)AddConfigurationSection("n2/host");
            EngineSection engineConfiguration = (EngineSection)AddConfigurationSection("n2/engine");
            if (hostConfiguration == null) throw new ConfigurationErrorsException("Couldn't find the n2/host configuration section. Please check the web configuration.");
            if (engineConfiguration == null) throw new ConfigurationErrorsException("Couldn't find the n2/engine configuration section. Please check the web configuration.");

            Url.DefaultExtension = hostConfiguration.Web.Extension;
            if (hostConfiguration.Web.IsWeb)
                webContext = new RequestContext();
            else
                webContext = new ThreadContext();
    
            DatabaseSection databaseConfiguration = (DatabaseSection)AddConfigurationSection("n2/database");
            AddConfigurationSection("n2/edit");

            host = AddComponentInstance<IHost>(new Host(webContext, hostConfiguration.RootID, hostConfiguration.StartPageID));
            AddComponentInstance<IWebContext>(webContext);

            IItemNotifier notifier = AddComponentInstance<IItemNotifier>(new ItemNotifier());
            ITypeFinder typeFinder = AddComponentInstance<ITypeFinder>(new MediumTrustTypeFinder(webContext, engineConfiguration));
            DefinitionBuilder definitionBuilder = AddComponentInstance<DefinitionBuilder>(new DefinitionBuilder(typeFinder));
			definitions = AddComponentInstance<IDefinitionManager>(new DefinitionManager(definitionBuilder, notifier));
			NHibernate.Cfg.Environment.UseReflectionOptimizer = false;
            IConfigurationBuilder nhBuilder = AddComponentInstance<IConfigurationBuilder>(new ConfigurationBuilder(definitions, databaseConfiguration));
            NHibernate.IInterceptor interceptor = AddComponentInstance<NHibernate.IInterceptor>(new NotifyingInterceptor(notifier));
            ISessionProvider sessionProvider = AddComponentInstance<ISessionProvider>(new SessionProvider(nhBuilder, interceptor, webContext));
            IItemFinder finder = AddComponentInstance<IItemFinder>(new ItemFinder(sessionProvider, definitions));
			INHRepository<int, ContentItem> itemRepository = AddComponentInstance<INHRepository<int, ContentItem>>(new NHRepository<int, ContentItem>(sessionProvider));
            INHRepository<int, LinkDetail> linkRepository = AddComponentInstance<INHRepository<int, LinkDetail>>(new NHRepository<int, LinkDetail>(sessionProvider));
            persister = AddComponentInstance<IPersister>(new ContentPersister(itemRepository, linkRepository, finder));

			if (hostConfiguration.MultipleSites)
			{
                ISitesProvider sitesProvider = AddComponentInstance<ISitesProvider>(new DynamicSitesProvider(persister, host.DefaultSite.RootItemID));
				urlParser = AddComponentInstance<IUrlParser>(new MultipleSitesParser(persister, webContext, notifier, host, sitesProvider, hostConfiguration));
			}
			else
			{
				urlParser = AddComponentInstance<IUrlParser>(new UrlParser(persister, webContext, notifier, host));
			}
            
            securityManager = AddComponentInstance<ISecurityManager>(new SecurityManager(webContext));
            ISecurityEnforcer securityEnforcer = AddComponentInstance<ISecurityEnforcer>(new SecurityEnforcer(persister, securityManager, urlParser, webContext));
            IVersionManager versioner = AddComponentInstance<IVersionManager>(new VersionManager(persister, itemRepository));
			N2.Edit.Settings.NavigationSettings settings = AddComponentInstance<N2.Edit.Settings.NavigationSettings>(new N2.Edit.Settings.NavigationSettings(webContext));
            IPluginFinder pluginFinder = new PluginFinder(typeFinder);
            editManager = AddComponentInstance<IEditManager>(new EditManager(typeFinder, definitions, persister, versioner, securityManager, pluginFinder, settings));
            integrityManager = AddComponentInstance<IIntegrityManager>(new IntegrityManager(definitions, urlParser));
            IIntegrityEnforcer integrityEnforcer = AddComponentInstance<IIntegrityEnforcer>(new IntegrityEnforcer(persister, integrityManager));
            rewriter = AddComponentInstance<IUrlRewriter>(new UrlRewriter(urlParser, webContext));
            IErrorHandler errorHandler = AddComponentInstance<IErrorHandler>(new ErrorHandler());
            lifeCycleHandler = AddComponentInstance<IRequestLifeCycleHandler>(new RequestLifeCycleHandler(rewriter, securityEnforcer, sessionProvider, webContext, errorHandler));
            ItemXmlReader xmlReader = AddComponentInstance<ItemXmlReader>(new ItemXmlReader(definitions));
            Importer importer = AddComponentInstance<Importer>(new GZipImporter(persister, xmlReader));
            ItemXmlWriter xmlWriter = AddComponentInstance<ItemXmlWriter>(new ItemXmlWriter(definitions, urlParser));
			AddComponentInstance<Exporter>(new GZipExporter(xmlWriter));
            AddComponentInstance<InstallationManager>(new InstallationManager(host, definitions, importer, persister, sessionProvider, nhBuilder));
            AddComponentInstance<ILanguageGateway>(new LanguageGateway(persister, finder, editManager, definitions, host, securityManager, webContext));
            AddComponentInstance<IPluginBootstrapper>(new PluginBootstrapper(typeFinder));
            AddComponentInstance<Navigator>(new Navigator(persister, host));

			AttributeExplorer<N2.Edit.Settings.IServiceEditable> serviceExplorer = new AttributeExplorer<N2.Edit.Settings.IServiceEditable>();
			AttributeExplorer<IEditableContainer> containerExplorer = new AttributeExplorer<IEditableContainer>();
			N2.Edit.Settings.SettingsManager settingsManager = new N2.Edit.Settings.SettingsManager(serviceExplorer, containerExplorer);
			EditableHierarchyBuilder<N2.Edit.Settings.IServiceEditable> hierarchyBuilder = new EditableHierarchyBuilder<N2.Edit.Settings.IServiceEditable>();
            AddComponentInstance<N2.Edit.Settings.ISettingsProvider>(new N2.Edit.Settings.SettingsProvider(settingsManager, hierarchyBuilder));
            AjaxRequestDispatcher dispatcher = AddComponentInstance<AjaxRequestDispatcher>(new AjaxRequestDispatcher(securityManager));
            CreateUrlProvider cup = AddComponentInstance<CreateUrlProvider>(new CreateUrlProvider(persister, editManager, definitions, dispatcher));
            ItemDeleter id = AddComponentInstance<ItemDeleter>(new ItemDeleter(persister, dispatcher));
            EditUrlProvider eud = AddComponentInstance<EditUrlProvider>(new EditUrlProvider(persister, editManager, dispatcher));
            ItemMover im = AddComponentInstance<ItemMover>(new ItemMover(persister, dispatcher));
            ItemCopyer ic = AddComponentInstance<ItemCopyer>(new ItemCopyer(persister, dispatcher));
            AddComponentInstance<ICacheManager>(new CacheManager(webContext, persister, hostConfiguration));

            foreach (KeyValuePair<Type, object> pair in resolves)
            {
                settingsManager.Handle(pair.Key.Name, pair.Value.GetType());
            }
        }

        private object AddConfigurationSection(string sectionName)
        {
            object section = WebConfigurationManager.GetSection(sectionName);
            if (section != null)
                AddComponentInstance(section.GetType().FullName, section.GetType(), section);
            return section;
        }

		#region Properties
		public IPersister Persister
		{
			get { return persister; }
		}

		public IUrlParser UrlParser
		{
			get { return urlParser; }
		}

		public IUrlRewriter Rewriter
		{
			get { return rewriter; }
		}

		public IDefinitionManager Definitions
		{
			get { return definitions; }
		}

		public IIntegrityManager IntegrityManager
		{
			get { return integrityManager; }
		}

		public ISecurityManager SecurityManager
		{
			get { return securityManager; }
		}

		public IEditManager EditManager
		{
			get { return editManager; }
		}

        public IHost Host
        {
            get { return host; }
        }

		public IDictionary<Type, object> Resolves
		{
			get { return resolves; }
		}
		#endregion

		#region Methods
		public void Initialize()
		{
			Debug.WriteLine("MediumTrustEngine: initializing plugins");

			IPluginBootstrapper invoker = Resolve<IPluginBootstrapper>();
			invoker.InitializePlugins(this, invoker.GetPluginDefinitions());
		}

		public void Attach(HttpApplication application)
		{
			lifeCycleHandler.Init(application);
		}

		public T Resolve<T>() where T : class
		{
			return Resolve(typeof(T)) as T;
		}

		public object Resolve(Type serviceType)
		{
			if (Resolves.ContainsKey(serviceType))
				return Resolves[serviceType];
			else
				throw new N2Exception("Couldn't find any service of the type " + serviceType);
		}


		public object Resolve(string key)
		{
			foreach (KeyValuePair<Type, object> pair in resolves)
			{
				if (pair.Key.Name == key)
					return pair.Value;
			}
			return null;
		}

		public void AddComponent(string key, Type classType)
		{
			AddComponent(key, classType, classType);
		}

		public void AddComponent(string key, Type serviceType, Type classType)
		{
			ConstructorInfo constructor = FindBestCosntructor(classType);
			object[] parameters = CreateConstructorParameters(constructor.GetParameters());
			object componentInstance = constructor.Invoke(parameters);
			
			AddComponentInstance(key, serviceType, componentInstance);
		}

		protected virtual object[] CreateConstructorParameters(ParameterInfo[] parameterInfos)
		{
			object[] parameters = new object[parameterInfos.Length];
			for (int i = 0; i < parameterInfos.Length; i++)
			{
				parameters[i] = Resolve(parameterInfos[i].ParameterType);
			}
			return parameters;
		}

		protected virtual ConstructorInfo FindBestCosntructor(Type classType)
		{
			int maxParameters = -1;
			ConstructorInfo bestContructor = null;
			foreach (ConstructorInfo constructor in classType.GetConstructors())
			{
				int parameters = constructor.GetParameters().Length;
				if (parameters > maxParameters)
				{
					bestContructor = constructor;
					maxParameters = parameters;
				}
			}

			return bestContructor;
		}

		public void AddFacility(string key, object facility)
		{
			Trace.WriteLine("MediumTrustEngine.AddFacility not implemented: " + key);
		}

		public void Release(object instance)
		{
			Trace.WriteLine("MediumTrustEngine.Release not implemented: " + instance);
		}

		[Obsolete("Use AddComponentInstance")]
		public void AddComponent(Type serviceType, object instance)
		{
			AddComponentInstance(serviceType.FullName, serviceType, instance);
		}
        private T AddComponentInstance<T>(T instance)
        {
            AddComponentInstance(typeof(T).Name, typeof(T), instance);
            return instance;
        }
		public void AddComponentInstance(string key, Type serviceType, object instance)
		{
			Resolves[serviceType] = instance;
			if (instance is IAutoStart)
			{
				(instance as IAutoStart).Start();
			}
		}

		#endregion
    }
}
