using System.Security.Principal;

namespace N2.Tests
{
    public static class SecurityUtilities
    {
        public static IPrincipal CreatePrincipal(string name, params string[] roles)
        {
            return new GenericPrincipal(new GenericIdentity(name, "TestIdentity"), roles);
        }
    }
}
