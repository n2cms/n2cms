namespace N2.Persistence
{
    /// <summary>
    /// Fulfils the dependency of an object. This class is used by the <see cref="ContentDependencyInjector"/>.
    /// </summary>
    public interface IDependencySetter
    {
        /// <summary>Injects dependencies onto an object inheriting from <see cref="IDependentEntity"/></summary>
        /// <param name="instance">The instance to inject the dependency on.</param>
        void Fulfil(object instance);
    }
}
