using System.Collections.Generic;
using N2.Definitions;
using N2.Persistence;
using N2.Integrity;


namespace N2.Security.Items
{
    /// <summary>
    /// Describes user's external login
    /// <seealso cref="Microsoft.AspNet.Identity.UserLoginInfo"/>
    /// </summary>
    [PartDefinition("External login",
        Description = "User's external login info")]
    [RestrictParents(typeof(User))]
    [Throwable(AllowInTrash.No)]
    [Versionable(AllowVersions.No)]
    public class UserExternalLoginInfo : ContentItem
    {
        public virtual string LoginProvider 
        { 
            get { return GetDetail<string>("LoginProvider", null); } 
            set { base.SetDetail<string>("LoginProvider", value, null); } 
        }
        
        public virtual string ProviderKey 
        { 
            get { return GetDetail<string>("ProviderKey", null); } 
            set { base.SetDetail<string>("ProviderKey", value, null); } 
        }

        public override string IconUrl
        {
            get 
            {
                /* TODO: icon may identify provider
                 * TODO: where to store provider icons?  User defined with fallback to N2 built-in icons? 
                 * 
                string vpath = N2.Web.Url.ResolveTokens(string.Format("{ManagementUrl}/Resources/Icons/{0}.png", LoginProvider));
                if (System.Web.Hosting.HostingEnvironment.VirtualPathProvider.FileExists(vpath))
                    return vpath;
                 */
                return base.IconUrl; 
            }
        }

        public override string Name
        {
            get 
            {
                /* TODO: define meaningfull name value
                 * 
                 return base.ToValidNameString(string.Format("{0}-{1}", LoginProvider, ProviderKey));
                 */
                return base.Name; 
            }
            set { base.Name = value; }
        }

        /// <summary> Returns query parameter to select a login </summary>
        /// <remarks> Hides implementation details (parameter vs. detail implementation) </remarks>
        public static Parameter[] QueryLoginInfoParameter(string loginProvider, string providerKey, ContentItem parent = null)
        {
            var parameters = new List<Parameter>();
            if (parent != null)
                parameters.Add(Parameter.Equal("Parent", parent));
            parameters.AddRange(new Parameter[] 
	            {
	                Parameter.TypeEqual(typeof(UserExternalLoginInfo)),
	                Parameter.Equal("LoginProvider", loginProvider).Detail(true),
	                Parameter.Equal("ProviderKey", providerKey).Detail(true)
	            });
            return parameters.ToArray();
        }
    }
}
