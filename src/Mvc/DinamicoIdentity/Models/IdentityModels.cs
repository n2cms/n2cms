using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using N2;
using N2.Definitions;
using N2.Integrity;
using N2.Security.AspNet.Identity;


namespace DinamicoIdentity.Models
{
    /// <summary> N2 application user </summary>
    /// <remarks>
    /// You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    /// </remarks>
    [PageDefinition("ApplicationUser", IconClass = "n2-icon-user")]
    [RestrictParents(typeof(N2.Security.Items.UserList))]
    [Throwable(AllowInTrash.No)]
    [Versionable(AllowVersions.No)]
    public class ApplicationUser : ContentUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, int> manager)
        {
            // N2 backward compatibility: generate security stamp when undefined yet
            if (string.IsNullOrEmpty(this.SecurityStamp))
            {
                manager.UpdateSecurityStamp(this.Id);
                this.SecurityStamp = manager.FindById(this.Id).SecurityStamp;
            }

            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            ClaimsIdentity userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            // See also: http://forums.asp.net/t/1973959.aspx?ASP+NET+MVC+5+OWIN+Missing+added+claims
            return userIdentity;
        }
    }

    /*
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
    */


    /// <summary> This is just to preseve Startup.Auth pattern </summary>
    public class ApplicationDbContext : IDisposable
    {
        public ApplicationDbContext()
        {
        }

        public void Dispose()
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    /*
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
    */
}