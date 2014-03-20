using N2.Configuration;
using N2.Definitions;
using N2.Definitions.Static;
using N2.Edit;
using N2.Edit.FileSystem;
using N2.Edit.Installation;
using N2.Edit.Settings;
using N2.Edit.Workflow;
using N2.Integrity;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Persistence.NH;
using N2.Persistence.NH.Finder;
using N2.Persistence.Serialization;
using N2.Plugin;
using N2.Plugin.Scheduling;
using N2.Security;
using N2.Web;
using N2.Web.UI;
using NHibernate;
using NHibernate.Cfg;
using N2.Engine.MediumTrust;
using N2.Edit.Versioning;

namespace N2.Engine.Configuration
{
    public class StaticDefaultServiceContainerConfigurer : IServiceContainerConfigurer
    {
        #region IServiceContainerConfigurer Members

        public void Configure(IEngine engine, EngineSection engineConfig)
        {
            Environment.UseReflectionOptimizer = false;

            engine.Container.AddComponentInstance("n2.engine", typeof(IEngine), engine);
            engine.Container.AddComponentInstance("n2.engineConfig", typeof(EngineSection), engineConfig);
            engine.Container.AddComponent("n2.stateChanger", typeof(StateChanger), typeof(StateChanger));
            engine.Container.AddComponent("n2.webContext", typeof(IWebContext), typeof(AdaptiveContext));
            engine.Container.AddComponent("n2.typeFinder", typeof(ITypeFinder), typeof(MediumTrustTypeFinder));
            engine.Container.AddComponent("n2.adapterProvider", typeof(IContentAdapterProvider), typeof(ContentAdapterProvider));
            engine.Container.AddComponent("n2.pluginBootstrapper", typeof(IPluginBootstrapper), typeof(PluginBootstrapper));
            engine.Container.AddComponent("n2.classMappingGenerator", typeof(ClassMappingGenerator), typeof(ClassMappingGenerator));
            engine.Container.AddComponent("n2.configurationBuilder", typeof(ConfigurationBuilder), typeof(ConfigurationBuilder));
            engine.Container.AddComponent("n2.sessionFactorySource", typeof(IConfigurationBuilder), typeof(ConfigurationSource));
            engine.Container.AddComponent("n2.itemNotifier", typeof(IItemNotifier), typeof(NHInterceptor));
            engine.Container.AddComponent("n2.interceptor", typeof(IInterceptor), typeof(NHInterceptor));
            engine.Container.AddComponent("n2.sessionProvider", typeof(ISessionProvider), typeof(SessionProvider));
            engine.Container.AddComponent("n2.repository", typeof(IRepository<>), typeof(NHRepository<>));
#pragma warning disable 612, 618
            engine.Container.AddComponent("n2.repository", typeof(IRepository<,>), typeof(NHRepository<,>)); // obsolete
            engine.Container.AddComponent("n2.repository.nh", typeof(INHRepository<>), typeof(NHRepository<>));
#pragma warning restore 612, 618
            engine.Container.AddComponent("n2.versioning", typeof(IVersionManager), typeof(VersionManager));
            engine.Container.AddComponent("n2.persister", typeof(IPersister), typeof(ContentPersister));
            engine.Container.AddComponent("n2.itemFinder", typeof(IItemFinder), typeof(ItemFinder));
            //trail tracker
            engine.Container.AddComponent("n2.attributeExplorer", typeof(AttributeExplorer), typeof(AttributeExplorer));
            engine.Container.AddComponent("n2.editableHierarchyBuilder", typeof(EditableHierarchyBuilder), typeof(EditableHierarchyBuilder));
            engine.Container.AddComponent("n2.definitions", typeof(IDefinitionManager), typeof(DefinitionManager));
            engine.Container.AddComponent("n2.definitionBuilder", typeof(DefinitionBuilder), typeof(DefinitionBuilder));
            engine.Container.AddComponent("n2.host", typeof(IHost), typeof(Host));
            engine.Container.AddComponent("n2.sitesProvider", typeof(ISitesProvider), typeof(DynamicSitesProvider));
            engine.Container.AddComponent("n2.htmlFilter", typeof(HtmlFilter), typeof(HtmlFilter));
            engine.Container.AddComponent("n2.requestPathAnalyzer", typeof(RequestPathProvider), typeof(RequestPathProvider));
            engine.Container.AddComponent("n2.ajaxRequestDispatcher", typeof(AjaxRequestDispatcher), typeof(AjaxRequestDispatcher));
            engine.Container.AddComponent("n2.cacheManager", typeof(ICacheManager), typeof(CacheManager));
            engine.Container.AddComponent("n2.security", typeof(ISecurityManager), typeof(SecurityManager));
            engine.Container.AddComponent("n2.safecontentrenderer", typeof(HtmlSanitizer), typeof(HtmlSanitizer));
            engine.Container.AddComponent("n2.securityEnforcer", typeof(ISecurityEnforcer), typeof(SecurityEnforcer));
            engine.Container.AddComponent("n2.fileSystem", typeof(IFileSystem), typeof(VirtualPathFileSystem));
            engine.Container.AddComponent("n2.defaultDirectory", typeof(IDefaultDirectory), typeof(DefaultDirectorySelector));
            engine.Container.AddComponent("n2.directorySelector", typeof(IEditManager), typeof(EditManager));
            engine.Container.AddComponent("n2.edit.navigator", typeof(Navigator), typeof(Navigator));
            engine.Container.AddComponent("n2.treeSorter", typeof(ITreeSorter), typeof(TreeSorter));
            engine.Container.AddComponent("n2.edit.navigationSettings", typeof(NavigationSettings), typeof(NavigationSettings));
            engine.Container.AddComponent("n2.heart", typeof(IHeart), typeof(Heart));
            engine.Container.AddComponent("n2.integrity", typeof(IIntegrityManager), typeof(IntegrityManager));
            engine.Container.AddComponent("n2.integrityEnforcer", typeof(IIntegrityEnforcer), typeof(IntegrityEnforcer));
            engine.Container.AddComponent("n2.installer", typeof(InstallationManager), typeof(InstallationManager));
            engine.Container.AddComponent("n2.errorHandler", typeof(IErrorNotifier), typeof(ErrorHandler));

            engine.Container.AddComponent("n2.itemXmlWriter", typeof(ItemXmlWriter), typeof(ItemXmlWriter));
            engine.Container.AddComponent("n2.itemXmlReader", typeof(ItemXmlReader), typeof(ItemXmlReader));

            engine.Container.AddComponent("n2.exporter", typeof(Exporter), typeof(GZipExporter));
            engine.Container.AddComponent("n2.importer", typeof(Importer), typeof(GZipImporter));

            engine.Container.AddComponent("n2.worker", typeof(IWorker), typeof(AsyncWorker));

            engine.Container.AddComponent("n2.requestHandler", typeof(RequestLifeCycleHandler), typeof(RequestLifeCycleHandler));
            engine.Container.AddComponent("n2.pluginFinder", typeof(IPluginFinder), typeof(PluginFinder));
            engine.Container.AddComponent("n2.scheduler", typeof(Scheduler), typeof(Scheduler));

            engine.Container.AddComponentInstance("n2.serviceContainer", typeof(IServiceContainer), engine.Container);
            engine.Container.AddComponent("n2.serviceRegistrator", typeof(ServiceRegistrator), typeof(ServiceRegistrator));
        }

        #endregion
    }
}
