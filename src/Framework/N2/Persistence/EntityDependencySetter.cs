using N2.Engine;

namespace N2.Persistence
{
    [Service]
    public class EntityDependencySetter<T> : IDependencySetter
    {
        T dependency;

        public EntityDependencySetter(T dependency)
        {
            this.dependency = dependency;
        }

        public void Fulfil(object entity)
        {
            var dependet = entity as IInjectable<T>;
            if (dependet != null)
                dependet.Set(dependency);
        }
    }
}
