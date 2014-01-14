using NHibernate;

namespace N2.Persistence.NH
{
    /// <summary>
    /// Builds NHibernate configuration and session factories.
    /// </summary>
    public interface IConfigurationBuilder
    {
        /// <summary>Builds the NHibernate configuration object.</summary>
        /// <returns>A populated NHibernate configuration.</returns>
        NHibernate.Cfg.Configuration BuildConfiguration();

        /// <summary>Build the NHibernate session factory. This is a costly operation.</summary>
        /// <returns>A session factory ready to serve sessions.</returns>
        ISessionFactory BuildSessionFactory();
    }
}
