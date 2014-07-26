using System;
using System.Linq;
using System.Collections.Generic;
using N2.Definitions;
using N2.Edit;
using N2.Engine;
using N2.Integrity;
using N2.Persistence;
using N2.Security;
using N2.Web;
using N2.Plugin;
using N2.Persistence.Sources;
using N2.Persistence.Proxying;

namespace N2.Tests.Fakes
{
    public class FakeEngine : IEngine
    {
        public FakeServiceContainer container = new FakeServiceContainer();

        public FakeEngine()
        {
        }

        public FakeEngine(params Type[] types)
        {
            AddComponentInstance<ITypeFinder>(new FakeTypeFinder(types));
            var definitionManager = TestSupport.SetupDefinitions(types.Where(t => typeof(ContentItem).IsAssignableFrom(t)).ToArray());
            AddComponentInstance<IDefinitionManager>(definitionManager);
            var adapterProvider = new ContentAdapterProvider(this, Resolve<ITypeFinder>());
            AddComponentInstance<IContentAdapterProvider>(adapterProvider);
            var itemRepository = new FakeContentItemRepository();
            AddComponentInstance<IRepository<ContentItem>>(itemRepository);
            AddComponentInstance<IContentItemRepository>(itemRepository);
            var webContext = new ThreadContext();
            AddComponentInstance<IWebContext>(webContext);
            var host = new Host(webContext, 1, 1);
            AddComponentInstance<IHost>(host);
            var security = new FakeSecurityManager();
            AddComponentInstance<ISecurityManager>(security);
            var source = new ContentSource(security, new [] { new DatabaseSource(host, itemRepository) });
            AddComponentInstance(source);
            AddComponentInstance<IPersister>(new ContentPersister(source, itemRepository));
            AddComponentInstance<IWebContext>(webContext);
            var proxyFactory = new InterceptingProxyFactory();
            AddComponentInstance<IProxyFactory>(proxyFactory);
            var activator = new ContentActivator(new N2.Edit.Workflow.StateChanger(), new ItemNotifier(), proxyFactory);
            AddComponentInstance<ContentActivator>(activator);
            activator.Initialize(definitionManager.GetDefinitions());
        }

        #region IEngine Members

        public N2.Persistence.IPersister Persister
        {
            get { return container.Resolve<IPersister>(); }
        }

        public N2.Web.IUrlParser UrlParser
        {
            get { return container.Resolve<IUrlParser>(); }
        }

        public IDefinitionManager Definitions
        {
            get { return container.Resolve<IDefinitionManager>(); }
        }

        public N2.Integrity.IIntegrityManager IntegrityManager
        {
            get { return container.Resolve<IIntegrityManager>(); }
        }

        public N2.Security.ISecurityManager SecurityManager
        {
            get { return container.Resolve<ISecurityManager>(); }
        }

        public N2.Edit.IEditManager EditManager
        {
            get { return container.Resolve<IEditManager>(); }
        }

        public N2.Edit.IEditUrlManager ManagementPaths
        {
            get { return container.Resolve<IEditUrlManager>(); }
        }

        public N2.Web.IWebContext RequestContext
        {
            get { return container.Resolve<IWebContext>(); }
        }

        public N2.Web.IHost Host
        {
            get { return container.Resolve<IHost>(); }
        }

        public IServiceContainer Container
        {
            get { return container; }
        }

        public void Initialize()
        {
            container.StartComponents();
        }

        public void Attach(EventBroker application)
        {
            throw new NotImplementedException();
        }

        public T Resolve<T>() where T : class
        {
            return Container.Resolve<T>();
        }

        public object Resolve(Type serviceType)
        {
            return Container.Resolve(serviceType);
        }

        public object Resolve(string key)
        {
            throw new NotImplementedException();
        }

        [Obsolete("Use Container.AddComponent")]
        public void AddComponent(string key, Type serviceType)
        {
            AddComponent(key, serviceType, serviceType);
        }

        [Obsolete("Use Container.AddComponent")]
        public void AddComponent(string key, Type serviceType, Type classType)
        {
            AddComponentInstance(key, serviceType, Activator.CreateInstance(classType));
        }

        [Obsolete("Use Container.AddComponentInstance")]
        public void AddComponentInstance(string key, Type serviceType, object instance)
        {
            container.AddComponentInstance(key, serviceType, instance);
        }

        [Obsolete("Use Container.AddComponentLifeStyle")]
        public void AddComponentLifeStyle(string key, Type serviceType, ComponentLifeStyle lifeStyle)
        {
            container.AddComponent(key, serviceType, serviceType);
        }

        [Obsolete("Not supportable by all service containers. Use the specific IServiceContainer implementation", true)]
        public void AddFacility(string key, object facility)
        {
            throw new NotImplementedException();
        }

        [Obsolete("Use Container.Release")]
        public void Release(object instance)
        {
            throw new NotImplementedException();
        }

        public ContentHelperBase Content
        {
            get { return new ContentHelperBase(() => this, () => RequestContext.CurrentPath); }
        }

        public N2.Configuration.ConfigurationManagerWrapper Config
        {
            get { return Resolve<N2.Configuration.ConfigurationManagerWrapper>(); }
        }

        #endregion

        public class FakeServiceContainer : IServiceContainer
        {
            readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

            #region IServiceContainer Members

            public void AddComponent(string key, Type serviceType, Type classType)
            {
                try
                {
                    services[serviceType] = Activator.CreateInstance(classType);
                }
                catch (Exception e)
                {
                    // the lack of constructor injection in this fake may cause unit tests to fail
                    // at the time of container configuration, let's just emit warnings in this step
                    // resolution will obviuosly fail when attempted
                    Logger.Warn(string.Format("FakeServiceContainer cannot resolve {0} to {1} : {2}", 
                        serviceType.FullName, classType.FullName, e.Message));
                }
            }

            public void AddComponentInstance(string key, Type serviceType, object instance)
            {
                services[serviceType] = instance;
            }

            public void AddComponentLifeStyle(string key, Type serviceType, ComponentLifeStyle lifeStyle)
            {
                throw new NotImplementedException();
            }

            public void AddComponentWithParameters(string key, Type serviceType, Type classType, IDictionary<string, string> properties)
            {
                throw new NotImplementedException();
            }

            public T Resolve<T>() where T: class
            {
                return (T) Resolve(typeof (T));
            }

            public T Resolve<T>(string key) where T : class
            {
                return (T) Resolve(typeof (T));
            }

            public object Resolve(Type type)
            {
                if (services.ContainsKey(type) == false)
                    throw new InvalidOperationException("No component for service " + type.Name + " registered");

                return services[type];
            }

            public void Release(object instance)
            {
            }

            public IEnumerable<object> ResolveAll(Type serviceType)
            {
                if (!this.services.ContainsKey(serviceType))
                    return new object[0];

                return new object[] { Resolve(serviceType) };
            }

            public IEnumerable<ServiceInfo> Diagnose()
            {
                yield break;
            }

            public IEnumerable<T> ResolveAll<T>() where T: class
            {
                if (!this.services.ContainsKey(typeof(T)))
                    return new T[0];

                return new T[] { Resolve<T>() };
            }

            public N2.Engine.Configuration.IServiceContainerConfigurer ServiceContainerConfigurer
            {
                get { throw new NotImplementedException(); }
            }

            public void StartComponents()
            {
                foreach (var component in this.services.Values.OfType<IAutoStart>().ToList())
                    component.Start();
            }

            #endregion
        }

        public void AddComponentInstance<T>(T instance)
        {
            Container.AddComponentInstance(instance.GetType().FullName, typeof(T), instance);
        }
    }
}
