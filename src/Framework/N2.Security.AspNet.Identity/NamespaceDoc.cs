
namespace N2.Security.AspNet.Identity
{
    /// <summary>
    /// Aspnet.Identity/Owin account subsystem for N2 CMS
    /// 
    /// A note on Aspnet.Identity:
    ///   it replaces classic Membership subsystem (System.Web.Security.Membership).
    ///   The most notable benefits of the new subsystem are:
    ///    - support for external authentication providers (Google, Facebook etc),
    ///    - a member may be authenticated at several external providers,
    ///    - local username/password credentials are supported,
    ///    - optional two factor authentication is supported etc .. 
    /// 
    /// A note on this particular implementation:
    ///   The implementation of Aspnet.Identity/Owin for N2CMS 
    ///   stores user accounts as N2 ContentItems, stored in existing N2 database schema.
    ///   Data structure is built on top of existing N2 membership subsystem
    ///   to be as much as possible backward compatible.
    ///   Migration procedure is required to convert Items.User local accounts:
    ///     - existing database user records have to be updated:
    ///       User derived class name should be updated to new user defined ApplicationUser class,
    ///     - stored passwords have to be invalidated
    ///       (password hashing is changed at new Identity subsystem). 
    ///   
    /// How to activate ASP.NET Identity:
    ///   N2 CMS code by default activates classic-membership subsystem to be back compatible.
    ///   Activating ASP.NET Identity subsystem is pretty simple:
    ///     - N2.Security.AspNet.Identity assembly should be dropped to application bin folder
    ///       (N2 loads all assemblies found there)
    ///     - loaded assembly replaces existing N2 membership logics (by IoC injection).
    ///     
    ///   See Dinamico.AspNet.Identity.Sample 
    ///   that demonstrates how to setup a N2 CMS based on ASP.NET Identity:
    ///     - create Visual Studio 2013 project based on ASP.NET Identity,
    ///     - remove default Entity Framework membership store,
    ///     - activate N2 ASP.NET Identity,
    ///     - and do some project final touches.
    /// 
    ///   Ready to go N2 CMS based on ASP.NET Identity
    ///     - todo.
    ///     
    ///   See also: 
    ///     <seealso cref="HowTo_SeeAlso">_Docs/HowTo_SeeAlso list of resources </seealso>
    ///     <seealso cref="HowTo_MvcProject">_Docs/HowTo_MvcProject how to setup a N2 Dinamico on Visual Studio MVC project </seealso>
    ///     <seealso cref="ReleaseNotes">_Docs/ReleaseNotes notes on N2.Security.AspNet.Identity </seealso>
    ///     
    /// </summary>
    public class NamespaceDoc
    {
    }
}
