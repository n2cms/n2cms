using System;
namespace N2.Persistence.NH
{
	public interface ISessionProvider: IDisposable
	{
		NHibernate.ISession GetOpenedSession();
		void Flush();
	}
}
