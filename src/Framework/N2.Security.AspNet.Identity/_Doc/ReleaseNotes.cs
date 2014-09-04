
namespace N2.Security.AspNet.Identity
{
    /// <summary>
    /// N2 Aspnet.Identity release notes
    /// <ul>
    ///  <li>2014.08.04 - janpub, JH4 pull request: N2 Identity lib and sample app (Visual Studio 2013 Update 2) <br/>
    ///  <em>N2CMS Dinamico on ASP.NET Identity.
    ///      To run DinamicoIdentity you should add N2 and Dinamico subfolders.
    ///      See also: NamespaceDoc (N2.Security.AspNet.Identity)</em>
    ///  </li>
    ///  <li>2014.08.04 - janpub, JH3 pull request: N2 (non-breaking) changes to support Identity <br/>
    ///   <em>
    ///   This completes N2 changes to support ASP.NET Identity:
    ///   a) N2 user management pages may be replaced with custom implementations
    ///   b) all calls to classic membership are wrapped.
    ///      Default implementation supports classic-membership to assure no breaking changes are introduced
    ///      (please review JH 3.4 commit).
    ///      TODO: wrap n2 installation logics! (It is still unchanged - and classic-membership specific).
    ///   </em>
    ///   <pre>
    ///   JH 3.1 - ItemBridge: additional methods to centralize logics (user roles management, IsAdmin)
    ///                        and to support upcoming Identity.
    ///   JH 3.2 - AccountResources: allows to replace N2 Management user management pages with custom implementations
    ///            MembershipAccountResources: default implementation. 
    ///            All references to management pages are replaces with token expressions,
    ///            e.g. "{Account.ManageUser.PageUrl}".ResolveUrlTokens().
    ///   JH 3.3 - AccountManager: allows to replace membership subsystem.
    ///            MembershipAccountManager: default classic-membership implementation.
    ///            All calls to classic-membership API are replaces, e.g.
    ///            System.Web.Security.Membership.FindUsersByName(..) is replaces with AccountManager.FindUserByName(..)
    ///   JH 3.4 - EditableRolesAttribute is moved from Framewrok to Management project
    ///   JH 3.5 - Fixing StartPageController: AccoutManager wrapps calls to System.Web.Roles
    ///   JH 3.6 - Dinamico LogOn menu gets link to "change password" page
    ///   JH 3.7 - Dinamico PermissionDenied handler redirect to custom or default login page
    ///   </pre>
    ///  </li>
    ///  <li>2014.07.14 - janpub, JH2 pull request submitted: Basic N2 User subsystem extensions 
    ///      https://github.com/n2cms/n2cms/pull/544
    ///  </li>
    ///  <li>2014.07.12 - janpub, JH1 pull request accepted: Basic code enhancements - Token values may contain tokens
    ///      https://github.com/n2cms/n2cms/commit/456f5b817063f62c796a536acf6a4417c5a108e1
    ///  </li>
    ///  <li>2013.12.05 - janpub, Description published </li>
    ///  <li>2013.12.05 - janpub, proto app on server http://ae.abaeda.miga.eu </li>
    ///  <li>2013.12.04 - janpub, N2 Dinamico + N2 Management running, alpha quality (unsupported: N2 installation)</li>
    ///  <li>2013.12.01 - janpub, N2 Dinamico in MVC5/Aspnet.Identity running, alpha quality (unsupported: N2 management, N2 installation) </li>
    ///  <li>2013.11.26 - janpub, Start of N2 Aspnet.Identity project <see cref="https://n2cms.codeplex.com/discussions/471621"/> </li>
    /// </ul>
    /// </summary>
    internal class ReleaseNotes
    {
    }
}
