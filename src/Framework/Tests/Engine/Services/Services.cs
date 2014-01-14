using N2.Engine;

namespace N2.Tests.Engine.Services
{
    public interface IBarometer
    {
        int GetPressure();
    }

    [Service(typeof(IBarometer), Configuration = "Low")]
    public class LowService : IBarometer
    {
        #region IBarometer Members

        public int GetPressure()
        {
            return 1;
        }

        #endregion
    }

    [Service(typeof(IBarometer), Configuration = "High")]
    public class HighService : IBarometer
    {
        #region IBarometer Members

        public int GetPressure()
        {
            return 2;
        }

        #endregion
    }

    public class BarometerWatcher
    {
        public BarometerWatcher(IBarometer[] barometers)
        {
            Barometers = barometers;
        }

        public IBarometer[] Barometers { get; set; }
    }

    public class AltitudeComparer
    {
        public AltitudeComparer(IBarometer[] barometers)
        {
            Barometers = barometers;
        }

        public IBarometer[] Barometers { get; set; }
    }

    [Service(Key = "Sesame")]
    public class SelfService
    {
    }

    [Service]
    public class DependingService
    {
        public SelfService service;
        public DependingService(SelfService service)
        {
            this.service = service;
        }
    }

    public class UnregisteredDependency
    {
    }

    [Service]
    public class DependingServiceWithMissingDependency
    {
        public UnregisteredDependency service;
        public DependingServiceWithMissingDependency(UnregisteredDependency service)
        {
            this.service = service;
        }
    }

    public interface IGenericSelfService<T>
    {
    }

    [Service]
    [Service(typeof(IGenericSelfService<>))]
    public class GenericSelfService<T> : IGenericSelfService<T>
    {
    }

    [Service]
    public class DependingGenericSelfService<T>
    {
        public SelfService service;
        public DependingGenericSelfService(SelfService service)
        {
            this.service = service;
        }
    }

    [Service]
    public class GenericDependingService
    {
        public GenericSelfService<int> service;
        public GenericDependingService(GenericSelfService<int> service)
        {
            this.service = service;
        }
    }

    [Service]
    public class GenericDependingOnInterfaceService
    {
        public IGenericSelfService<int> service;
        public IGenericSelfService<string> service2;
        public GenericDependingOnInterfaceService(IGenericSelfService<int> service, IGenericSelfService<string> service2)
        {
            this.service = service;
            this.service2 = service2;
        }
    }

    public interface IService
    {
    }

    [Service(typeof(IService))]
    public class InterfacedService : IService
    {
    }

    [Service(typeof(IService), Replaces = typeof(InterfacedService))]
    public class ReplacingService : IService
    {
    }

    public interface IReplacedInterface
    {
    }

    [Service(typeof(IReplacedInterface))]
    public class ReplacedService : IReplacedInterface
    {
    }

    [Service(typeof(IReplacedInterface), Replaces = typeof(IReplacedInterface))]
    public class ReplacingReplacedService : IReplacedInterface
    {
    }

    public class ConcreteService : AbstractService
    {
    }

    public abstract class AbstractService : IService
    {
    }

    [Service(typeof(IService))]
    public class DecoratingService : IService
    {
        public IService decorated;

        public DecoratingService(IService decorated)
        {
            this.decorated = decorated;
        }
    }


    public class OuterService
    {
        [Service]
        public class InnerService
        {
        }
    }

    public class OuterService2 : OuterService
    {
        [Service]
        public new class InnerService
        {
        }
    }

    public interface IInternalService
    {
    }

    [Service(typeof(IInternalService))] 
    internal class InternalService : IInternalService
    {
    }

    public interface IGenericService<T>
    {
    }

    [Service(typeof(IGenericService<>))]
    public class GenericInterfacedService<T> : IGenericService<T>
    {
    }

    [Service]
    public class GenericInterfaceDependingService
    {
        public IGenericService<int> service;
        public GenericInterfaceDependingService(IGenericService<int> service)
        {
            this.service = service;
        }
    }
    
    public class NonAttributed
    {
    }

}
