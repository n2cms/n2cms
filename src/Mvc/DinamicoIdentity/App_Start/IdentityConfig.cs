using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

using N2.Engine;
using N2.Security;
using N2.Security.AspNet.Identity;
using DinamicoIdentity.Models;

namespace DinamicoIdentity
{
    /// <summary> N2 account resources (replaces default classic-membership service) </summary>
    [Service(typeof(AccountResources), Replaces = typeof(MembershipAccountResources))]
    public class ApplicationAccountResources : IdentityAccountResources
    {
    }


    /// <summary> N2 account manager service(replaces default classic-membership service) </summary>
    [Service(typeof(AccountManager), Replaces = typeof(MembershipAccountManager))]
    public class ApplicationAccountManager : IdentityAccountManager<ApplicationUser>
    {
        public ApplicationAccountManager(
            IdentityUserStore<ApplicationUser> userStore, ApplicationUserManager userManager,
            IdentityRoleStore roleStore, IdentityRoleManager roleManager)
            : base(userStore, userManager, roleStore, roleManager)
        {
        }
    }


    /// <summary> N2 Identity role manager </summary>
    [Service(typeof(IdentityRoleManager))]
    public class ApplicationRoleManager : IdentityRoleManager
    {
        public ApplicationRoleManager(IdentityRoleStore store)
            : base(store)
        {
        }
    }


    /// <summary>UserManager on IdentityUSerStore </summary>
    /// <remarks>
    /// Configure the application user manager used in this application. 
    /// UserManager is defined in ASP.NET Identity and is used by the application.
    /// </remarks>
    [Service]
    public class ApplicationUserManager : IdentityUserManager<ApplicationUser>
    {
        public ApplicationUserManager(IdentityUserStore<ApplicationUser> store)
            : base(store)
        {
            Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            // Don's dispose underlying storage!
            // (See HowTo_MvcProject.cs notes)
            // base.Dispose(disposing);
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var store = N2.Context.Current.Resolve<IdentityUserStore<ApplicationUser>>();
            var manager = new ApplicationUserManager(store);
            return manager;
        }

        private void Initialize() 
        {
            ApplicationUserManager manager = this;

            // This is to support pre-identity password hashes:
            manager.PasswordHasher = new CompatibilityModePasswordHasher();

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser, int>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<ApplicationUser, int>
            {
                MessageFormat = "Your security code is: {0}"
            });
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser, int>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is: {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();

            /*
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser, int>(dataProtectionProvider.Create("N2 ASP.NET Identity"));
            }
            */
           
        }
    }

    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your sms service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
