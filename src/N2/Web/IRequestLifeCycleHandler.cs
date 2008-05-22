using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace N2.Web
{
	/// <summary>
	/// Classes implementing this interface are eligible to handle the request 
	/// life cycle for N2.
	/// </summary>
	public interface IRequestLifeCycleHandler
	{
		/// <summary>Subscribes to applications events.</summary>
		/// <param name="application">The application.</param>
		void Init(HttpApplication application);
	}
}
