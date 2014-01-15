namespace N2.Plugin
{
    /// <summary>Classes implementing this interface can serve as plug in initializers. If one of these classes is referenced by a PlugInAttribute it's initialize methods will be invoked by the N2 factory during initialization.</summary>
    public interface IPluginInitializer
    {
        /// <summary>Invoked after the factory has been initialized.</summary>
        /// <param name="engine">The factory that has been initialized.</param>
        void Initialize(Engine.IEngine engine);
    }
}
