using System.Web;
using N2.Engine;
using System;

namespace N2.Web
{
	/// <summary>
	/// Classes implementing this interface are eligible to handle the request 
	/// life cycle for N2.
	/// </summary>
	public interface IRequestLifeCycleHandler
	{
		[Obsolete]
		void Initialize();
	}
}
