
namespace N2.Security.AspNet.Identity
{
    /// <summary>
    ///     N2 Aspnet.Identity see also (with comments)
    /// 
    /// 
    ///     Overview
    ///     http://www.asp.net/identity
    ///     
    ///     MVC 5 App with Facebook .. OAuth2 Sign-on 
    ///     (how to configure MVC application)
    ///     http://www.asp.net/mvc/tutorials/mvc-5/create-an-aspnet-mvc-5-app-with-facebook-and-google-oauth2-and-openid-sign-on
    ///     http://www.asp.net/web-api/overview/security/external-authentication-services
    ///     
    ///     Authentication and Identity
    ///     http://www.asp.net/aspnet/overview/authentication-and-identity
    /// 
    ///     Owin and Katana
    ///     http://www.asp.net/aspnet/overview/owin-and-katana
    /// 
    ///     OWIN Startup Class Detection 
    ///     http://www.asp.net/aspnet/overview/owin-and-katana/owin-startup-class-detection
    ///     (see also Startup.cs)
    ///     
    ///     Adding ASP.NET Identity to a Web Forms Project
    ///     http://www.asp.net/identity/overview/getting-started/adding-aspnet-identity-to-an-empty-or-existing-web-forms-project
    ///     A sample: Web Forms and Identity
    ///     https://aspnet.codeplex.com/SourceControl/latest#Samples/Identity/Webforms.Samples/Readme.txt
    ///     
    ///     Additional Owin Oauth Providers (e.g. LinkedIn)
    ///     https://github.com/owin-middleware/OwinOAuthProviders 
    ///     
    ///     NHibernate.AspNet.Identity
    ///     https://github.com/milesibastos/NHibernate.AspNet.Identity
    ///     
    ///     MongoDB.AspNet.Identity
    ///     http://www.nuget.org/packages/MongoDB.AspNet.Identity/
    /// 
    ///     
    ///     http://www.asp.net/identity/overview/releases
    ///     http://blogs.msdn.com/b/webdev/archive/2014/03/20/test-announcing-rtm-of-asp-net-identity-2-0-0.aspx
    ///     An overview what's new in Identity 2.0
    ///       Two-factor authentication, Account lockout, Account (email) confirmation, 
    ///       Password reset, Security stamp (to invalidate issued security cookies),
    ///       Enhanced Password Validator,
    ///       Users Primary Key Type is configurable (e.g. can be of int type),
    ///       IQueryable on users and roles,
    ///       UserManager may delete an User.
    ///     Note: release note describes default implementation on Entity Framework.
    ///       Guidelines how to implement Identity on other subsystems might be derived from the writing.
    ///       Unique keys:
    ///         User Id is a primary key field, Username gets unique key in underlying store 
    ///         "to ensure that Usernames are always unique and there was no race condition in which you could end up with duplicate usernames."
    ///       A new and preferred way how to register and access UserManager is introduced:  
    ///         StartupAuth.cs includes registration of a factory method:
    ///         <![CDATA[
    ///            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
    ///         ]]>
    ///         This is a recommended way of getting an instance of UserManager **per request** for the application:
    ///         <![CDATA[
    ///            HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
    ///         ]]>     
    ///     
    ///     Per request lifetime management for UserManager class in ASP.NET Identity
    ///     http://blogs.msdn.com/b/webdev/archive/2014/02/12/per-request-lifetime-management-for-usermanager-class-in-asp-net-identity.aspx
    ///        StartupAuth.cs includes UserManager registration to
    ///        ".. with this approach it is possible to configure the properties on the UserManager, such as password length and complexity."
    ///        
    ///        Identity 1.0 implemented on EF had a problem
    ///        ".. In the current approach, if there are two instances of the UserManager in the request that work on the same user, they would be working with two different instances of the user object."
    ///        ".. The solution to the above problem is to store a single instance of UserManager and DbContext per request and reuse them throughout the application."
    ///        Note: N2 on ItemBridge does not share the problem.
    ///      
    /// 
    /// </summary>
    internal class HowTo_SeeAlse
    {
    }
}
