using System;
using System.Collections.Generic;
using System.Web;
using System.Reflection;
using System.Diagnostics;
using N2.Persistence;
using N2.Plugin.Scheduling;
using N2.Web;
using N2.Definitions;
using N2.Integrity;
using N2.Security;
using N2.Edit;
using N2.Persistence.NH;
using N2.Persistence.NH.Finder;
using N2.Serialization;
using N2.Details;
using N2.Persistence.Finder;
using N2.Plugin;
using N2.Web.Parts;
using N2.Configuration;
using N2.Engine.Globalization;
using System.Web.Configuration;
using N2.Installation;
using System.Configuration;
using N2.Web.UI;

namespace N2.Engine.MediumTrust
{
	public class MediumTrustEngine : IEngine
	{
        ISecurityManager securityManager;
        IDefinitionManager definitions;
        IUrlParser urlParser;
        IEditManager editManager;
        IIntegrityManager integrityManager;
        IHost host;
        IRequestLifeCycleHandler lifeCycleHandler;
        IPersister persister;
		IWebContext webContext;

        IDictionary<Type, object> resolves = new Dictionary<Type, object>();
            
		public MediumTrustEngine()
		{
            EditSection editConfiguration = (EditSection)AddConfigurationSection("n2/edit");
            DatabaseSection databaseConfiguration = (DatabaseSection)AddConfigurationSection("n2/database");
            if (databaseConfiguration == null) throw new ConfigurationErrorsException("Couldn't find the n2/database configuration section. Please check the web configuration.");
            HostSection hostConfiguration = (HostSection)AddConfigurationSection("n2/host");
            if (hostConfiguration == null) throw new ConfigurationErrorsException("Couldn't find the n2/host configuration section. Please check the web configuration.");
            EngineSection engineConfiguration = (EngineSection)AddConfigurationSection("n2/engine");
            if (engineConfiguration == null) throw new ConfigurationErrorsException("Couldn't find the n2/engine configuration section. Please check the web configuration.");

            RegisterConfiguredComponents(engineConfiguration);
            
            Url.DefaultExtension = hostConfiguration.Web.Extension;
		    webContext = new AdaptiveContext();
    
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
                ISitesProvider sitesProvider = AddComponentInstance<ISitesProvider>(new DynamicSitesProvider(persister, host, hostConfiguration));
				urlParser = AddComponentInstance<IUrlParser>(new MultipleSitesParser(persister, webContext, notifier, host, sitesProvider, hostConfiguration));
			}
			else
				urlParser = AddComponentInstance<IUrlParser>(new UrlParser(persister, webContext, notifier, host, hostConfiguration));
			
			if (hostConfiguration.Web.Urls.EnableCaching)
				urlParser = new CachingUrlParserDecorator(urlParser, persister);
            
            securityManager = AddComponentInstance<ISecurityManager>(new SecurityManager(webContext));
            ISecurityEnforcer securityEnforcer = AddComponentInstance<ISecurityEnforcer>(new SecurityEnforcer(persister, securityManager, urlParser, webContext));
            IVersionManager versioner = AddComponentInstance<IVersionManager>(new VersionManager(itemRepository, finder));
			N2.Edit.Settings.NavigationSettings settings = AddComponentInstance<N2.Edit.Settings.NavigationSettings>(new N2.Edit.Settings.NavigationSettings(webContext));
            IPluginFinder pluginFinder = AddComponentInstance<IPluginFinder>(new PluginFinder(typeFinder));
            editManager = AddComponentInstance<IEditManager>(new EditManager(definitions, persister, versioner, securityManager, pluginFinder, settings, editConfiguration));
            integrityManager = AddComponentInstance<IIntegrityManager>(new IntegrityManager(definitions, urlParser));
            IIntegrityEnforcer integrityEnforcer = AddComponentInstance<IIntegrityEnforcer>(new IntegrityEnforcer(persister, integrityManager));
            ItemXmlReader xmlReader = AddComponentInstance<ItemXmlReader>(new ItemXmlReader(definitions));
            ItemXmlWriter xmlWriter = AddComponentInstance<ItemXmlWriter>(new ItemXmlWriter(definitions, urlParser));
            Importer importer = AddComponentInstance<Importer>(new GZipImporter(persister, xmlReader));
            InstallationManager installer = AddComponentInstance<InstallationManager>(new InstallationManager(host, definitions, importer, persister, sessionProvider, nhBuilder));
            IErrorHandler errorHandler = AddComponentInstance<IErrorHandler>(new ErrorHandler(webContext, securityManager, installer, engineConfiguration));
			IRequestDispatcher dispatcher = AddComponentInstance<IRequestDispatcher>(new RequestDispatcher(this, webContext, urlParser, typeFinder, errorHandler, hostConfiguration));
			lifeCycleHandler = AddComponentInstance<IRequestLifeCycleHandler>(new RequestLifeCycleHandler(webContext, installer, dispatcher, editConfiguration));
            AddComponentInstance<Exporter>(new GZipExporter(xmlWriter));
            AddComponentInstance<ILanguageGateway>(new LanguageGateway(persister, finder, editManager, definitions, host, securityManager, webContext));
            AddComponentInstance<IPluginBootstrapper>(new PluginBootstrapper(typeFinder));
            AddComponentInstance<Navigator>(new Navigator(persister, host));

            AjaxRequestDispatcher ajaxDispatcher = AddComponentInstance<AjaxRequestDispatcher>(new AjaxRequestDispatcher(securityManager));
            CreateUrlProvider cup = AddComponentInstance<CreateUrlProvider>(new CreateUrlProvider(persister, editManager, definitions, ajaxDispatcher));
            ItemDeleter id = AddComponentInstance<ItemDeleter>(new ItemDeleter(persister, ajaxDispatcher));
            EditUrlProvider eud = AddComponentInstance<EditUrlProvider>(new EditUrlProvider(persister, editManager, ajaxDispatcher));
            ItemMover im = AddComponentInstance<ItemMover>(new ItemMover(persister, ajaxDispatcher));
            ItemCopyer ic = AddComponentInstance<ItemCopyer>(new ItemCopyer(persister, ajaxDispatcher));
            AddComponentInstance<ICacheManager>(new CacheManager(webContext, persister, hostConfiguration));
            AddComponentInstance<ITreeSorter>(new TreeSorter(persister, editManager, webContext));

			IHeart heart = AddComponentInstance(new Heart(engineConfiguration));
			AddComponentInstance(new Scheduler(pluginFinder, heart, webContext, errorHandler));
        }

        private object AddConfigurationSection(string sectionName)
        {
            object section = WebConfigurationManager.GetSection(sectionName);
            if (section != null)
                AddComponentInstance(section.GetType().FullName, section.GetType(), section);
            return section;
        }

        private void RegisterConfiguredComponents(EngineSection engineConfig)
        {
            foreach (ComponentElement component in engineConfig.Components)
            {
                Type implementation = Type.GetType(component.Implementation);
                Type service = Type.GetType(component.Service) ?? implementation;
                if (service != null)
                    AddComponent(service.FullName, service, implementation);
            }
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

		public IWebContext RequestContext
		{
			get { return webContext; }
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

			AddComponentInstance<IEngine>(this);
			
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
            Trace.TraceError("MediumTrustEngine.AddFacility not implemented: " + key);
		}

		public void Release(object instance)
		{
            foreach (KeyValuePair<Type, object> pair in resolves)
            {
                if (pair.Value == instance)
                {
                    resolves.Remove(pair.Key);
                    break;
                }
            }
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
            if (Resolves.ContainsKey(serviceType))
                return;

			Resolves[serviceType] = instance;
			if (instance is IAutoStart)
			{
				(instance as IAutoStart).Start();
			}
		}

		#endregion
    }
}
