using System.Configuration;

namespace N2.Configuration
{
    /// <summary>
    /// Configuration related to membership, and the ContentMembershipProvider.
    /// </summary>
    public class MembershipElement : ConfigurationElement
    {
        /// <summary>A custom user type to instantiate instead of the default.</summary>
        [ConfigurationProperty("userType", DefaultValue = "N2.Security.Items.User, N2.Management")]
        public string UserType
        {
            get { return (string)base["userType"]; }
            set { base["userType"] = value; }
        }
    }
}
