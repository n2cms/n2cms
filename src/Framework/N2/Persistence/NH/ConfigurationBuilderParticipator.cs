using N2.Engine;

namespace N2.Persistence.NH
{
    /// <summary>
    /// Modifies the NHibernate configuration object before the session factory is built.
    /// </summary>
    public abstract class ConfigurationBuilderParticipator
    {
        public abstract void AlterConfiguration(NHibernate.Cfg.Configuration cfg);
    }

    [Service(typeof(ConfigurationBuilderParticipator))]
    public class EmptyConfigurationBuilderParticipator : ConfigurationBuilderParticipator
    {
        public override void AlterConfiguration(NHibernate.Cfg.Configuration cfg)
        {
        }
    }
}
