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
    public class UserLogin : ContentItem
    {
        public virtual string LoginProvider { get { return GetDetail<string>("LoginProvider", null); } set { base.SetDetail<string>("LoginProvider", value, null); } }
        public virtual string ProviderKey   { get { return GetDetail<string>("ProviderKey", null); } set { base.SetDetail<string>("ProviderKey", value, null); } }

        /// <summary> Returns query parameter to select a login </summary>
        /// <remarks> Hides implementation details (parameter vs. detail) </remarks>
        public static Parameter[] QueryLoginInfoParameter(string loginProvider, string providerKey, ContentItem parent = null)
        {
            var parameters = new List<Parameter>();
            if (parent != null)
                parameters.Add(Parameter.Equal("Parent", parent));
            parameters.AddRange(new Parameter[] 
            {
                Parameter.TypeEqual(typeof(UserLogin)),
                Parameter.Equal("LoginProvider", loginProvider).SetDetail(true),
                Parameter.Equal("ProviderKey", providerKey).SetDetail(true)
            });
            return parameters.ToArray();
        }
    }
}