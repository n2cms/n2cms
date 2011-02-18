using N2.Web.Mvc;

namespace N2.Definitions.Runtime
{
	public interface IRegistration
	{
		Builder<T> Register<T>(T container) where T : IContainable;
	}
}
