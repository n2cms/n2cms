namespace N2.Plugin
{
    /// <summary>
    /// Classes implementing this interface define plugins and are responsible of
    /// calling plugin initializers.
    /// </summary>
    public interface IPluginDefinition
    {
        /// <summary>Executes the plugin initializer.</summary>
        /// <param name="engine">A reference to the current engine.</param>
        void Initialize(Engine.IEngine engine);
    }
}
