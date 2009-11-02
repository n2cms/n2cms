using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using N2.Configuration;
using N2.Definitions;
using N2.Edit;
using N2.Edit.FileSystem;
using N2.Edit.Settings;
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
using N2.Web.UI;
using NHibernate;

namespace N2.Engine.MediumTrust
{
	public class MediumTrustServiceContainer : ServiceContainerBase
	{
		private readonly IDictionary<Type, object> container = new Dictionary<Type, object>();
		private readonly IDictionary<Type, Function<Type, object>> resolvers = new Dictionary<Type, Function<Type, object>>();

		#region IServiceContainer Members

		public override void AddFacility(string key, object facility)
		{
			throw new NotImplementedException();
		}

		public override void AddComponentLifeStyle(string key, Type serviceType, ComponentLifeStyle lifeStyle)
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

		public override void AddComponentInstance(string key, Type serviceType, object instance)
		{
			if (resolvers.ContainsKey(serviceType))
				return;

			container[serviceType] = instance;
			resolvers[serviceType] = ReturnContainerInstance;

			if (instance is IAutoStart)
			{
				(instance as IAutoStart).Start();
			}
		}

		public override void AddComponent(string key, Type serviceType, Type classType)
		{
			CheckForAutoStart(key, serviceType, classType);

			RegisterSingletonResolver(key, serviceType, classType);
		}

		public override object Resolve(string key)
		{
			return Resolve(Type.GetType(key));
		}

		public override object Resolve(Type serviceType)
		{
			if (resolvers.ContainsKey(serviceType))
				return resolvers[serviceType](serviceType);

			if (serviceType.IsGenericType)
			{
				var baseDefinition = serviceType.GetGenericTypeDefinition();

				if (resolvers.ContainsKey(baseDefinition))
				{
					return resolvers[baseDefinition](serviceType);
				}
			}

			throw new N2Exception("Couldn't find any service of the type " + serviceType);
		}

		public override void Release(object instance)
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

		public override void Configure(IEngine engine, EngineSection engineConfig)
		{
			NHibernate.Cfg.Environment.UseReflectionOptimizer = false;

			AddComponentInstance("n2.engine", typeof(IEngine), engine);
			AddComponentInstance("n2.engineConfig", typeof(EngineSection), engineConfig);
			AddComponent("n2.webContext", typeof(IWebContext), typeof(AdaptiveContext));
			AddComponent("n2.typeFinder", typeof(ITypeFinder), typeof(MediumTrustTypeFinder));
			AddComponent("n2.aspectControllerProvider", typeof(IContentAdapterProvider), typeof(ContentAdapterProvider));
			AddComponent("n2.pluginBootstrapper", typeof(IPluginBootstrapper), typeof(PluginBootstrapper));
			AddComponent("n2.configurationBuilder", typeof(ConfigurationBuilder), typeof(ConfigurationBuilder));
			AddComponent("n2.sessionFactorySource", typeof(IConfigurationBuilder), typeof(ConfigurationSource));
			AddComponent("n2.itemNotifier", typeof(IItemNotifier), typeof(NotifyingInterceptor));
			AddComponent("n2.interceptor", typeof(IInterceptor), typeof(NotifyingInterceptor));
			AddComponent("n2.sessionProvider", typeof(ISessionProvider), typeof(SessionProvider));
			AddComponent("n2.repository", typeof(IRepository<,>), typeof(NHRepository<,>));
			AddComponent("n2.repository.nh", typeof(INHRepository<,>), typeof(NHRepository<,>));
			AddComponent("n2.versioning", typeof(IVersionManager), typeof(VersionManager));
			AddComponent("n2.persister", typeof(IPersister), typeof(ContentPersister));
			AddComponent("n2.itemFinder", typeof(IItemFinder), typeof(ItemFinder));
			AddComponent("n2.attributeExplorer", typeof(AttributeExplorer), typeof(AttributeExplorer));
			AddComponent("n2.editableHierarchyBuilder", typeof(EditableHierarchyBuilder), typeof(EditableHierarchyBuilder));
			AddComponent("n2.definitions", typeof(IDefinitionManager), typeof(DefinitionManager));
			AddComponent("n2.definitionBuilder", typeof(DefinitionBuilder), typeof(DefinitionBuilder));
			AddComponent("n2.host", typeof(IHost), typeof(Host));
			AddComponent("n2.sitesProvider", typeof(ISitesProvider), typeof(DynamicSitesProvider));
			AddComponent("n2.htmlFilter", typeof(HtmlFilter), typeof(HtmlFilter));
			AddComponent("n2.requestDispatcher", typeof(IRequestDispatcher), typeof(RequestDispatcher));
			AddComponent("n2.ajaxRequestDispatcher", typeof(AjaxRequestDispatcher), typeof(AjaxRequestDispatcher));
			AddComponent("n2.cacheManager", typeof(ICacheManager), typeof(CacheManager));
			AddComponent("n2.security", typeof(ISecurityManager), typeof(SecurityManager));
			AddComponent("n2.securityEnforcer", typeof(ISecurityEnforcer), typeof(SecurityEnforcer));
			AddComponent("n2.fileSystem", typeof(IFileSystem), typeof(VirtualPathFileSystem));
			AddComponent("n2.defaultDirectory", typeof(IDefaultDirectory), typeof(DefaultDirectorySelector));
			AddComponent("n2.directorySelector", typeof(IEditManager), typeof(EditManager));
			AddComponent("n2.edit.navigator", typeof(Navigator), typeof(Navigator));
			AddComponent("n2.treeSorter", typeof(ITreeSorter), typeof(TreeSorter));
			AddComponent("n2.edit.navigationSettings", typeof(NavigationSettings), typeof(NavigationSettings));
			AddComponent("n2.heart", typeof(IHeart), typeof(Heart));
			AddComponent("n2.integrity", typeof(IIntegrityManager), typeof(IntegrityManager));
			AddComponent("n2.integrityEnforcer", typeof(IIntegrityEnforcer), typeof(IntegrityEnforcer));
			AddComponent("n2.installer", typeof(InstallationManager), typeof(InstallationManager));
			AddComponent("n2.errorHandler", typeof(IErrorHandler), typeof(ErrorHandler));

			AddComponent("n2.itemXmlWriter", typeof(ItemXmlWriter), typeof(ItemXmlWriter));
			AddComponent("n2.itemXmlReader", typeof(ItemXmlReader), typeof(ItemXmlReader));

			AddComponent("n2.exporter", typeof(Exporter), typeof(GZipExporter));
			AddComponent("n2.importer", typeof(Importer), typeof(GZipImporter));

			AddComponent("n2.worker", typeof(IWorker), typeof(AsyncWorker));

			AddComponent("n2.requestHandler", typeof(IRequestLifeCycleHandler), typeof(RequestLifeCycleHandler));
			AddComponent("n2.pluginFinder", typeof(IPluginFinder), typeof(PluginFinder));
			AddComponent("n2.scheduler", typeof(Scheduler), typeof(Scheduler));

			//<include uri="assembly://N2/Serialization/serialization.castle.config" />
		}

		public override void StartComponents()
		{
			foreach (var resolver in resolvers)
			{
				CheckForAutoStart(resolver.Key.FullName.ToLowerInvariant(), resolver.Key, resolver.Key);
			}
		}

		#endregion

		private object ReturnContainerInstance(Type serviceType)
		{
			if (!container.ContainsKey(serviceType))
				throw new N2Exception("Couldn't find any service of the type " + serviceType);

			return container[serviceType];
		}

		private void CheckForAutoStart(string key, Type serviceType, Type classType)
		{
			foreach (Type t in classType.GetInterfaces())
			{
				if (t == typeof(IAutoStart))
					container[serviceType] = CreateInstance(serviceType, classType, key);
			}
		}

		private void RegisterSingletonResolver(string key, Type serviceType, Type classType)
		{
			resolvers[serviceType] = delegate(Type type)
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
			resolvers[serviceType] = delegate(Type type) { return CreateInstance(type, classType, key); };
		}

		protected object CreateInstance(Type serviceType, Type classType, string key)
		{
			if (classType.ContainsGenericParameters)
			{
				var arguments = serviceType.GetGenericArguments();
				classType = classType.MakeGenericType(arguments);
			}

			var constructorInfo = FindBestConstructor(classType);

			if (constructorInfo.ConstructorInfo == null)
			{
				StringBuilder errorMessage = new StringBuilder("Could not find resolvable constructor for class " + classType.FullName);

				foreach(var parameter in constructorInfo.CouldNotFindParameters)
				{
					errorMessage.AppendLine("\nCould not resolve " + parameter.ParameterType);
				}

				throw new N2Exception(errorMessage.ToString());
			}
			var constructor = constructorInfo.ConstructorInfo;

			object[] parameters = CreateConstructorParameters(constructor.GetParameters());
			object componentInstance = constructor.Invoke(parameters);
			AddComponentInstance(key, serviceType, componentInstance);
			return componentInstance;
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

		protected virtual ConstructorFindInfo FindBestConstructor(Type classType)
		{
			int maxParameters = -1;
			ConstructorInfo bestConstructor = null;
			ParameterInfo[] couldNotFindParameters = null;
			foreach (ConstructorInfo constructor in classType.GetConstructors())
			{
				var parameters = constructor.GetParameters();
				var couldNotFindParametersTemp = ResolveAllParameters(parameters);
				if (parameters.Length <= maxParameters || couldNotFindParametersTemp.Length != 0)
				{
					couldNotFindParameters = couldNotFindParametersTemp;
					continue;
				}

				bestConstructor = constructor;
				maxParameters = parameters.Length;
				couldNotFindParameters = new ParameterInfo[0];
			}

			return new ConstructorFindInfo
			       	{
			       		ConstructorInfo = bestConstructor,
						CouldNotFindParameters = couldNotFindParameters
			       	};
		}

		public class ConstructorFindInfo
		{
			public ConstructorInfo ConstructorInfo { get; set; }

			public ParameterInfo[] CouldNotFindParameters { get; set; }
		}

		private ParameterInfo[] ResolveAllParameters(IEnumerable<ParameterInfo> parameters)
		{
			var result = new List<ParameterInfo>();
			foreach (var parameter in parameters)
			{
				if (!resolvers.ContainsKey(parameter.ParameterType) &&
					(!parameter.ParameterType.IsGenericType ||
					 !resolvers.ContainsKey(parameter.ParameterType.GetGenericTypeDefinition())))
				{
					result.Add(parameter);
				}
			}
			return result.ToArray();
		}
	}
}
