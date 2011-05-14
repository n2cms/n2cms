using N2.Engine;
using NHibernate;

namespace N2.Persistence.NH
{
	/// <summary>
	/// An unncessary abstraction that stores the single session factory 
	/// instance for whoomever may need it.
	/// </summary>
	[Service(typeof(IConfigurationBuilder))]
	public class ConfigurationSource : IConfigurationBuilder
	{
		readonly ConfigurationBuilder builder;
		ISessionFactory factory;
		NHibernate.Cfg.Configuration cfg;

		public ConfigurationSource(ConfigurationBuilder builder)
		{
			this.builder = builder;
		}

		public virtual NHibernate.Cfg.Configuration BuildConfiguration()
		{
			lock (this)
			{
				if (cfg == null)
					cfg = builder.BuildConfiguration();

				return cfg;
			}
		}

		public virtual ISessionFactory BuildSessionFactory()
		{
			lock(this)
			{
				if (factory == null)
					factory = BuildConfiguration().BuildSessionFactory();
				
				return factory;
			}
		}
	}
}