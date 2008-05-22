using System;
namespace N2.Persistence.NH
{
	public interface IConfigurationBuilder
	{
		NHibernate.Cfg.Configuration BuildConfiguration();
		NHibernate.ISessionFactory BuildSessionFactory();
	}
}
