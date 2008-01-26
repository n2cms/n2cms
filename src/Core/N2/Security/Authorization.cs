using System.Security.Principal;

namespace N2.Security
{
	/// <summary>
	/// In future versions this class may have additional implementations.
	/// </summary>
	public abstract class Authorization
	{
		public abstract bool IsAuthorized(IPrincipal user);
	}
}
