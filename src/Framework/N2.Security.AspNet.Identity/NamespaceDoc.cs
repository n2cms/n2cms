
namespace N2.Security.AspNet.Identity
{
    /// <summary>
    /// <h2>Aspnet.Identity/Owin account subsystem for N2 CMS</h2>
    /// <para>
    /// <em>Aspnet.Identity replaces Membership subsystem, introducing support for external authentication providers (Google, Facebook etc)
    ///     in parallel with traditional local (username,password) accounts. 
    ///     Accounts are stored as N2 ContentItems, local accounts are backward compatible. 
    ///     Migration procedure is required to convert Items.User local accounts 
    ///     to user defined ApplicationUser instances (derived from Items.User).
    /// </em>
    /// </para>
    /// 
    /// <h3>(Non) breaking changes</h3>
    /// <em>N2 and N2.Management is changes to be agnostic to account subsystem. 
    ///     <see cref="ReleaseNotes"/>
    /// </em>
    /// <ul>
    ///  <li>Membership <br/>
    ///    <em> Default account implementation is (still) Membership based.
    ///     There's no breaking changes for WebForms/Membership or MVC4/Membership 
    ///     based N2 applications. The same account system agnostic N2 codebase
    ///     allows parallel development lifecycle for Membership and new Aspnet.Identity based N2 applications.</em>
    ///  </li>
    ///  <li>Aspnet.Identity <br/>
    ///    <em> It's new promising subsystem with many features not found in old system,
    ///     however by time of witing this document it's on bleeding edge.
    ///     There are some breaking changes (obviously FormsAuthentication and Membership API should not be used).
    ///    </em>
    ///  </li>
    /// </ul>
    /// 
    /// <seealso cref="ReleaseNotes"/> Release notes, links to resources, breaking change warnings.
    /// <seealso cref="HowTo_MvcProject"/> How to setup new Visual Studio MVC project and create N2 CMS application.
    /// <seealso cref="HowTo_Git"/> How to setup development environment
    /// <seealso cref="HowIs_Implemented"/> An overview how N2 Aspnet.Identity is implemented in coexistence with Membership
    /// <seealso cref="http://www.asp.net/identity/overview/getting-started/introduction-to-aspnet-identity"/>
    /// </summary>
    public class NamespaceDoc
    {
    }
}
