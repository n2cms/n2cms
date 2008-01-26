using System;
namespace N2.Security
{
	public interface ISecurityEnforcer
	{
		void AuthorizeRequest(N2.Web.IWebContext context);
		void Start();
	}
}
