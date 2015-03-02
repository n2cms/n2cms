using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace N2.Security.AspNet.Identity
{
    /// <summary> Backward compatible password hasher </summary>
    /// <remarks> Accepts old classic-membership hashed passwords and new Identity hashed passwords.
    /// </remarks>
    public class CompatibilityModePasswordHasher : PasswordHasher 
    {
        private readonly N2.Engine.Logger<CompatibilityModePasswordHasher> logger;

        /// <summary> New passwords are hashed by Identity subsystem </summary>
        public override string HashPassword(string password)
        {
            return base.HashPassword(password);
        }

        /// <summary> Accepts passwords hashed by new Identity or by classic-membership subsystems </summary>
        public override PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            PasswordVerificationResult result;
            try
            {
                // Note: the method may throw errors on unexpected hashedPassword contents
                result = base.VerifyHashedPassword(hashedPassword, providedPassword);
            }
            catch (Exception ex)
            {
                result = PasswordVerificationResult.Failed;
                logger.Warn(ex);
            }

            if (result == PasswordVerificationResult.Failed)
            {
                // classic-membership hashing: (clear and encrypted passwords not supported)
                // System.Web assembly needed:
                string hashed = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(providedPassword, System.Web.Configuration.FormsAuthPasswordFormat.SHA1.ToString());
                if (hashedPassword == hashed)
                {
                    result = PasswordVerificationResult.SuccessRehashNeeded;
                    // see also: https://aspnetidentity.codeplex.com/workitem/1928
                }
            }
            return result;
        }

    }
}
