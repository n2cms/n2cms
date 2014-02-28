
namespace N2.Security.AspNet.Identity
{
    /// <summary>
    /// <h2>An overview how N2 Aspnet.Identity is implemented in coexistence with Membership</h2>
    /// <em> <see cref="https://n2cms.codeplex.com/discussions/471621"/> Initial description </em>
    /// 
    /// <h3>Existing codebase changes</h3>
    /// 
    /// <h4>N2 Framework changes</h4>
    /// Changes are just minor enhancements to N2 code:
    /// <ul>
    ///  <li><see cref="N2.Persistence.Parameter.SetDetail"/> allows chaining to simplify some expressions </li>
    ///  <li><see cref="N2.Web.Url.ResolveTokens"/> now supports token values that contains tokens. </li>
    ///  <li><see cref="N2.Details.EditableRolesAttribute"/> moved from N2 Framework to N2 Management. </li>
    /// </ul>
    /// 
    /// <h4>N2 Management changes (Part I)</h4>
    /// Hardcoded paths to User/Role management pages are replaces with tokens.
    /// Token values may be redefined to support alternate User/Role management pages.
    /// Token default values point to existing N2 pages, therefore ne breaking changes are introduced.
    /// <ul>
    ///  <li><see cref="N2.Security.AccountResource"/> new tokens are defined by an autostart service. 
    ///  The service may be redefined or replaced to support alternate User/Role management pages. </li>
    ///  <li><see cref="N2.Management.Api.InterfaceBuilder"/> 
    ///      <see cref="N2.Management.ManagementItemsNodeAdapter"/>
    ///  now supports token values that contains tokens. </li>
    ///  <li><see cref="N2.Security.UserLogin"/> describes external logins (e.g. Google, Facebook etc). </li>
    /// </ul>
    /// 
    /// <h4>Dinamico changes (Part I)</h4>
    /// Hardcoded paths to Management endpoints replaced with tokens.
    /// <ul>
    ///  <li><code>Dinamico/Themes/Default/Views/Shared/LayoutPartials/LogOn.cshtml</code> <br/>
    ///    Hardcoded expression <code>@Url.Action("LogOff", "Membership")</code>
    ///    is replaced with token expression <code>"{Account.Logout.PageUrl}".ResolveUrlTokens()</code>
    ///  </li>
    ///  <li><code>MvcTemplates/N2/Top.master</code> <br/>
    ///    Hardcoded expression <code>Login.aspx?logout=true"</code>
    ///    is replaced with token expression <code>"{Account.Logout.PageUrl}".ResolveUrlTokens()</code></li>
    ///  <li>Note: any user-defined pages should use token expressions instead of hardcoded values aswell.</li>
    /// </ul>
    /// 
    /// <h4>N2 Management changes (Part II)</h4>
    /// Membership API calls are abstracted by new <see cref="N2.Security.AccountManager"/> service.
    /// There's no breaking changes for MVC4/Membership N2 applications - default service implementation
    /// performs unchanged calls to Membership subsystem.
    /// <ul>
    ///  <li><see cref=""/></li>
    ///  <li><see cref="System.Web.Security"/> API is membership based. AccountManager calls are used instead in N2 code. </li>
    ///  <li><see cref="N2.Security.AccountManager"/> new abstract service provides API similar to Membership </li>
    ///  <li><see cref="N2.Security.MembershipAccountManager"/> new service implementation encapsulates Membership API </li>
    ///  <li>N2 Management UI pages are now AccountManager based, e.g. <code>Mvc/MvcTemplates/N2/Users/Users.aspx</code>, <code>Edit.aspx</code>, 
    ///    <code>Password.aspx</code>, <code>Mvc\MvcTemplates\N2\Roles\Roles.aspx</code> etc.
    ///  </li>
    /// </ul>
    /// 
    ///
    /// <h3>N2 CMS on Aspnet.Identity</h3>
    /// 
    /// <h4>N2.Security.AspNet.Identity</h4>
    /// Is designed to be (almost) drop-in replacement of existing Membership subsystem.
    /// Provides implementation of <see cref="AccountResources"/> and <see cref="AccountManager"/> based 
    /// on new Aspnet.Identity account subsystem. <br/>
    /// See also <see cref="HowTo_MvcProject"/> how to setup a MVC5 Aspnet.Identity based N2 CMS application.
    /// Basically we just add new library to the project what replaces Membership subsystem services with Aspnet.Identity based. 
    /// <br/><br/>
    /// Key library components are:
    /// <ul>
    ///  <li><see cref="AspNetAccountResources"/> login/logout Urls are directed to Visual Studio generated AccountController </li>
    ///  <li><see cref="AspNetAccountManager"/> uses Aspnet.Identity UserStore and RoleStore instead of Membership subsystem </li>
    ///  <li><see cref="UserStore"/> Aspnet.Identity UserStore, implementen on top of N2 ItemBridge (accounts are stored as ContentItems). 
    ///    Local accounts are compatible with existing N2 account records.
    ///  </li>
    ///  <li><see cref="RoleStore"/> Aspnet.Identity RoleStore, implementen on top of N2 ItemBridge.
    ///    Compatible with existing N2 role records. </li>
    ///  <li><see cref="ContentUser"/> is base class representing user account (extended from Items.User).
    ///    Optional <see cref="N2.Security.UserLogin"/> children describes external logins. </li>
    /// </ul>
    /// 
    /// <h4>N2 CMS application</h4>
    /// Basically all we need is to add:
    /// <ul>
    ///  <li>N2 assembly references to a Visual Studio generated MVC5 application,</li>
    ///  <li>Web.config should receive N2 specific configurations,</li>
    ///  <li>ApplicationUser class to describe user account (optionally with additional properties) 
    ///      should be defined, extending <see cref="ContentUser"/> class,</li>
    ///  <li>Some minor adjustments in VisualStudio generated AccountController code are needed.</li>
    /// </ul>
    /// <see cref="HowTo_MvcProject"/> Detailed description how to setup N2 CMS from scratch.
    /// 
    /// </summary>
    internal class HowIs_Implemented
    {
    }
}
