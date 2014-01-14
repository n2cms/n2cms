using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using global::TinyIoC;
using N2.Plugin;

namespace N2.Engine.TinyIoC
{
    public class TinyIoCServiceContainer : IServiceContainer
    {
        private bool componentsStarted;
        public TinyIoCContainer Container { get; set; }

        public TinyIoCServiceContainer()
        {
            Container = new TinyIoCContainer();
        }

        public void AddComponent(string key, Type serviceType, Type classType)
        {
            Container.Register(serviceType, classType, key).AsSingleton();
            if (componentsStarted && typeof(IAutoStart).IsAssignableFrom(classType))
                (Container.Resolve(serviceType, key) as IAutoStart).Start();
        }

        public void AddComponentInstance(string key, Type serviceType, object instance)
        {
            Container.Register(serviceType, instance, key);
            if (componentsStarted && instance is IAutoStart)
                (instance as IAutoStart).Start();
        }

        public void AddComponentLifeStyle(string key, Type serviceType, ComponentLifeStyle lifeStyle)
        {
            var registration = Container.Register(serviceType, key);
            switch (lifeStyle)
            {
                case ComponentLifeStyle.Singleton:
                    registration.AsSingleton();
                    break;
                case ComponentLifeStyle.Transient:
                    registration.AsMultiInstance();
                    break;
            }
            if (componentsStarted && typeof(IAutoStart).IsAssignableFrom(serviceType))
                (Resolve(serviceType) as IAutoStart).Start();
        }

        public void AddComponentWithParameters(string key, Type serviceType, Type classType, IDictionary<string, string> properties)
        {
            throw new NotImplementedException();
        }

        public T Resolve<T>() where T : class
        {
            return Container.Resolve<T>();
        }

        public T Resolve<T>(string key) where T : class
        {
            return Container.Resolve<T>(key);
        }

        public object Resolve(Type type)
        {
            return Container.Resolve(type);
        }

        public void Release(object instance)
        {
        }

        public IEnumerable<object> ResolveAll(Type serviceType)
        {
            return Container.ResolveAll(serviceType);
        }

        public IEnumerable<ServiceInfo> Diagnose()
        {
            return Container.GetTypeRegistrations()
                .Select(tr => new ServiceInfo { ServiceType = tr.Type, ImplementationType = tr.Type, Key = tr.Name, Resolve = () => Resolve(tr.Type), ResolveAll = () => ResolveAll(tr.Type), ServiceTypes = new [] { tr.Type } });
        }

        public IEnumerable<T> ResolveAll<T>() where T : class
        {
            return Container.ResolveAll<T>();
        }

        public void StartComponents()
        {
            foreach (var component in Container.ResolveAllAssignableFrom<IAutoStart>())
                component.Start();
            componentsStarted = true;
        }
    }
}
