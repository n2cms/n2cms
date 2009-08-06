using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Web.Configuration;
using N2.Configuration;
using N2.Definitions;
using N2.Details;
using N2.Edit;
using N2.Edit.FileSystem;
using N2.Edit.Settings;
using N2.Engine.Globalization;
using N2.Installation;
using N2.Integrity;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Persistence.NH;
using N2.Persistence.NH.Finder;
using N2.Plugin;
using N2.Plugin.Scheduling;
using N2.Security;
using N2.Serialization;
using N2.Web;
using N2.Web.Parts;
using N2.Web.UI;
using NHibernate;
using Environment=NHibernate.Cfg.Environment;

namespace N2.Engine.MediumTrust
{
	public class MediumTrustEngine : IEngine
	{
		protected IDictionary<Type, object> container = new Dictionary<Type, object>();
		private IDefinitionManager definitions;
		private IEditManager editManager;
		private IHost host;
		private IIntegrityManager integrityManager;
		private IPersister persister;
		private IDictionary<Type, Function<Type, object>> resolvers = new Dictionary<Type, Function<Type, object>>();
		private ISecurityManager securityManager;
		private IUrlParser urlParser;
		private IWebContext webContext;

		public MediumTrustEngine(EventBroker broker)
		{
			var editConfiguration = (EditSection) AddConfigurationSection("n2/edit");
			var databaseConfiguration = (DatabaseSection) AddConfigurationSection("n2/database");
			if (databaseConfiguration == null)
				throw new ConfigurationErrorsException(
					"Couldn't find the n2/database configuration section. Please check the web configuration.");
			var hostConfiguration = (HostSection) AddConfigurationSection("n2/host");
			if (hostConfiguration == null)
				throw new ConfigurationErrorsException(
					"Couldn't find the n2/host configuration section. Please check the web configuration.");
			var engineConfiguration = (EngineSection) AddConfigurationSection("n2/engine");
			if (engineConfiguration == null)
				throw new ConfigurationErrorsException(
					"Couldn't find the n2/engine configuration section. Please check the web configuration.");
			var connectionStrings = (ConnectionStringsSection) AddConfigurationSection("connectionStrings");
			if (connectionStrings == null)
				throw new ConfigurationErrorsException(
					"Couldn't find the connectionStrings configuration section. Please check the web configuration.");

			RegisterConfiguredComponents(engineConfiguration);

			Url.DefaultExtension = hostConfiguration.Web.Extension;
			webContext = new AdaptiveContext();

			AddComponentInstance(broker);
			host = AddComponentInstance<IHost>(new Host(webContext, hostConfiguration.RootID, hostConfiguration.StartPageID));
			AddComponentInstance(webContext);

			var notifier = AddComponentInstance<IItemNotifier>(new ItemNotifier());
			var typeFinder = AddComponentInstance<ITypeFinder>(new MediumTrustTypeFinder(webContext, engineConfiguration));
			DefinitionBuilder definitionBuilder = AddComponentInstance(new DefinitionBuilder(typeFinder, engineConfiguration));
			definitions = AddComponentInstance<IDefinitionManager>(new DefinitionManager(definitionBuilder, notifier));
			Environment.UseReflectionOptimizer = false;
			ConfigurationBuilder nhBuilder =
				AddComponentInstance(new ConfigurationBuilder(definitions, databaseConfiguration, connectionStrings));
			var sessionFactorySource = AddComponentInstance<IConfigurationBuilder>(new ConfigurationSource(nhBuilder));
			var interceptor = AddComponentInstance<IInterceptor>(new NotifyingInterceptor(notifier));
			var sessionProvider =
				AddComponentInstance<ISessionProvider>(new SessionProvider(sessionFactorySource, interceptor, webContext));
			var finder = AddComponentInstance<IItemFinder>(new ItemFinder(sessionProvider, definitions));

			INHRepository<int, ContentItem> itemRepository = RegisterRepository<ContentItem>(sessionProvider);
			INHRepository<int, ContentDetail> detailRepository = RegisterRepository<ContentDetail>(sessionProvider);
			INHRepository<int, LinkDetail> linkRepository = RegisterRepository<LinkDetail>(sessionProvider);
			INHRepository<int, DetailCollection> collectionRepository = RegisterRepository<DetailCollection>(sessionProvider);
			INHRepository<int, AuthorizedRole> roleRepository = RegisterRepository<AuthorizedRole>(sessionProvider);

			persister = AddComponentInstance<IPersister>(new ContentPersister(itemRepository, linkRepository, finder));

			if (hostConfiguration.MultipleSites)
			{
				var sitesProvider =
					AddComponentInstance<ISitesProvider>(new DynamicSitesProvider(persister, host, hostConfiguration));
				urlParser =
					AddComponentInstance<IUrlParser>(new MultipleSitesParser(persister, webContext, notifier, host, sitesProvider,
					                                                         hostConfiguration));
			}
			else
				urlParser = AddComponentInstance<IUrlParser>(new UrlParser(persister, webContext, notifier, host, hostConfiguration));

			if (hostConfiguration.Web.Urls.EnableCaching)
				urlParser = new CachingUrlParserDecorator(urlParser, persister);

			securityManager = AddComponentInstance<ISecurityManager>(new SecurityManager(webContext, editConfiguration));
			var securityEnforcer =
				AddComponentInstance<ISecurityEnforcer>(new SecurityEnforcer(persister, securityManager, urlParser, webContext));
			var versioner = AddComponentInstance<IVersionManager>(new VersionManager(itemRepository, finder));
			NavigationSettings settings = AddComponentInstance(new NavigationSettings(webContext));
			var pluginFinder = AddComponentInstance<IPluginFinder>(new PluginFinder(typeFinder, engineConfiguration));
			editManager =
				AddComponentInstance<IEditManager>(new EditManager(definitions, persister, versioner, securityManager, pluginFinder,
				                                                   settings, editConfiguration));
			integrityManager = AddComponentInstance<IIntegrityManager>(new IntegrityManager(definitions, urlParser));
			var integrityEnforcer = AddComponentInstance<IIntegrityEnforcer>(new IntegrityEnforcer(persister, integrityManager));
			ItemXmlReader xmlReader = AddComponentInstance(new ItemXmlReader(definitions));
			ItemXmlWriter xmlWriter = AddComponentInstance(new ItemXmlWriter(definitions, urlParser));
			var importer = AddComponentInstance<Importer>(new GZipImporter(persister, xmlReader));
			InstallationManager installer =
				AddComponentInstance(new InstallationManager(host, definitions, importer, persister, sessionProvider,
				                                             sessionFactorySource));
			var errorHandler =
				AddComponentInstance<IErrorHandler>(new ErrorHandler(webContext, securityManager, installer, engineConfiguration));
			var aspectController = AddComponentInstance<IContentAdapterProvider>(new ContentAdapterProvider(this, typeFinder));
			var dispatcher =
				AddComponentInstance<IRequestDispatcher>(new RequestDispatcher(aspectController, webContext, urlParser, errorHandler,
				                                                               hostConfiguration));
			AddComponentInstance<IRequestLifeCycleHandler>(new RequestLifeCycleHandler(webContext, broker, installer, dispatcher,
			                                                                           errorHandler, editConfiguration,
			                                                                           hostConfiguration));
			AddComponentInstance<Exporter>(new GZipExporter(xmlWriter));
			AddComponentInstance<ILanguageGateway>(new LanguageGateway(persister, finder, editManager, definitions, host,
			                                                           securityManager, webContext));
			AddComponentInstance<IPluginBootstrapper>(new PluginBootstrapper(typeFinder, engineConfiguration));
			AddComponentInstance(new Navigator(persister, host));
			AddComponentInstance<IFileSystem>(new VirtualPathFileSystem());
			AddComponentInstance<IDefaultDirectory>(new DefaultDirectorySelector(host, editConfiguration));

			AjaxRequestDispatcher ajaxDispatcher = AddComponentInstance(new AjaxRequestDispatcher(securityManager));
			CreateUrlProvider cup =
				AddComponentInstance(new CreateUrlProvider(persister, editManager, definitions, ajaxDispatcher));
			ItemDeleter id = AddComponentInstance(new ItemDeleter(persister, ajaxDispatcher));
			EditUrlProvider eud = AddComponentInstance(new EditUrlProvider(persister, editManager, ajaxDispatcher));
			ItemMover im = AddComponentInstance(new ItemMover(persister, ajaxDispatcher));
			ItemCopyer ic = AddComponentInstance(new ItemCopyer(persister, ajaxDispatcher));
			AddComponentInstance<ICacheManager>(new CacheManager(webContext, persister, hostConfiguration));
			AddComponentInstance<ITreeSorter>(new TreeSorter(persister, editManager, webContext));

			IHeart heart = AddComponentInstance(new Heart(engineConfiguration));
			AddComponentInstance(new Scheduler(this, pluginFinder, heart, webContext, errorHandler));
		}

		public MediumTrustEngine() : this(EventBroker.Instance)
		{
		}

		private INHRepository<int, T> RegisterRepository<T>(ISessionProvider sessionProvider) where T : class
		{
			INHRepository<int, T> itemRepository = new NHRepository<int, T>(sessionProvider);
			AddComponentInstance(itemRepository);
			AddComponentInstance<IRepository<int, T>>(itemRepository);
			return itemRepository;
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

		public IDictionary<Type, object> Container
		{
			get { return container; }
		}

		public IDictionary<Type, Function<Type, object>> Resolvers
		{
			get { return resolvers; }
		}

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

		#endregion

		#region Methods

		public void Initialize()
		{
			Debug.WriteLine("MediumTrustEngine: initializing plugins");

			AddComponentInstance<IEngine>(this);

			var invoker = Resolve<IPluginBootstrapper>();
			invoker.InitializePlugins(this, invoker.GetPluginDefinitions());
		}

		public T Resolve<T>() where T : class
		{
			return Resolve(typeof (T)) as T;
		}

		public object Resolve(Type serviceType)
		{
			if (Resolvers.ContainsKey(serviceType))
				return Resolvers[serviceType](serviceType);
			throw new N2Exception("Couldn't find any service of the type " + serviceType);
		}


		public object Resolve(string key)
		{
			return Resolve(Type.GetType(key));
		}

		public virtual void AddComponent(string key, Type serviceType)
		{
			AddComponent(key, serviceType, serviceType);
		}

		public void AddComponent(string key, Type serviceType, Type classType)
		{
			CheckForAutoStart(key, serviceType, classType);

			RegisterSingletonResolver(key, serviceType, classType);
		}

		public void AddComponentLifeStyle(string key, Type serviceType, ComponentLifeStyle lifeStyle)
		{
			CheckForAutoStart(key, serviceType, serviceType);

			switch (lifeStyle)
			{
				case ComponentLifeStyle.Transient:
					RegisterTransientResolver(key, serviceType, serviceType);
					break;
				default:
					RegisterSingletonResolver(key, serviceType, serviceType);
					break;
			}
		}

		public void AddFacility(string key, object facility)
		{
			Trace.TraceError("MediumTrustEngine.AddFacility not implemented: " + key);
		}

		public void Release(object instance)
		{
			foreach (var pair in container)
			{
				if (pair.Value == instance)
				{
					container.Remove(pair.Key);
					break;
				}
			}
		}

		public void AddComponentInstance(string key, Type serviceType, object instance)
		{
			if (Resolvers.ContainsKey(serviceType))
				return;

			container[serviceType] = instance;
			Resolvers[serviceType] = ReturnContainerInstance;

			if (instance is IAutoStart)
			{
				(instance as IAutoStart).Start();
			}
		}

		private void RegisterSingletonResolver(string key, Type serviceType, Type classType)
		{
			Resolvers[serviceType] = delegate(Type type)
			                         	{
			                         		if (container.ContainsKey(type))
			                         			return container[type];

			                         		object componentInstance = CreateInstance(type, classType, key);
			                         		container[type] = componentInstance;
			                         		return componentInstance;
			                         	};
		}

		private void RegisterTransientResolver(string key, Type serviceType, Type classType)
		{
			Resolvers[serviceType] = delegate(Type type) { return CreateInstance(type, classType, key); };
		}

		private void CheckForAutoStart(string key, Type serviceType, Type classType)
		{
			foreach (Type t in classType.GetInterfaces())
			{
				if (t == typeof (IAutoStart))
					container[serviceType] = CreateInstance(serviceType, classType, key);
			}
		}

		protected object CreateInstance(Type serviceType, Type classType, string key)
		{
			ConstructorInfo constructor = FindBestConstructor(classType);
			object[] parameters = CreateConstructorParameters(constructor.GetParameters());
			object componentInstance = constructor.Invoke(parameters);
			AddComponentInstance(key, serviceType, componentInstance);
			return componentInstance;
		}

		private T AddComponentInstance<T>(T instance)
		{
			AddComponentInstance(typeof (T).Name, typeof (T), instance);
			return instance;
		}

		protected virtual object[] CreateConstructorParameters(ParameterInfo[] parameterInfos)
		{
			var parameters = new object[parameterInfos.Length];
			for (int i = 0; i < parameterInfos.Length; i++)
			{
				parameters[i] = Resolve(parameterInfos[i].ParameterType);
			}
			return parameters;
		}

		protected virtual ConstructorInfo FindBestConstructor(Type classType)
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

		private object ReturnContainerInstance(Type serviceType)
		{
			if (!container.ContainsKey(serviceType))
				throw new N2Exception("Couldn't find any service of the type " + serviceType);

			return container[serviceType];
		}

		#endregion
	}
}