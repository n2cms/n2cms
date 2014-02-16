using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using N2.Security.Items;
using N2.Definitions;
using N2.Details;
using N2.Integrity;
using N2.Persistence;

// review: (JH) is ID property good enough for UserId? Is better to use Guid.NewGuid().ToString() ?
// review: (JH) is UserName as Name good enough solution?  Name value restrictions should be propagated to end-users.
// review: (JH) is null password local account secure in all situations (e.g. when switched back to old-style membesrship)? 

namespace N2.Security.AspNet.Identity
{
    /// <summary> Aspnet.Identity user base class 
    /// <seealso cref="AspNetAccountManager<>"/>
    /// </summary>
    public abstract class ContentUser : User, IUser
    {
        private readonly N2.Engine.Logger<ContentUser> logger;

        public override string Title { get { return base.Title ?? base.Name; } set { base.Title = value; } }

        #region IUser, IUserStore

        /// <summary> UserId synonym (nonpersisted property) </summary>
        public string Id { get { return UserId; } set { UserId = value; } }

        /// <summary> Unique user id </summary>
        public string UserId { get { return base.ID.ToString(); } set { base.ID = (!string.IsNullOrEmpty(value) ? int.Parse(value) : 0); } }
        // { get { return GetDetail<string>("UserId", null); } set { base.SetDetail<string>("UserId", value, null); } }

        /// <summary> Returns query parameter to select user by userId </summary>
        public static Parameter UserIdQueryParameter(string userId) 
        {
            int id;
            if (!int.TryParse(userId, out id))
                id = -1;
            return Parameter.Equal("ID", id).SetDetail(false);
        }

        /// <summary> UserName and Name are synonyms </summary>
        public string UserName { get { return base.Name; } set { base.Name = value; } }

        #endregion

        #region IUserPasswordStore

        /// <summary> PasswordHash and Password are synonyms (no other stored password forms are supported) </summary>
        public string PasswordHash { get { return base.Password; } set { base.Password = value; } }

        #endregion

        #region IUserSecurityStampStore

        /// <summary> Represent snapshot of user credentials. Guid value, re-generated on user property changes. </summary>
        public string SecurityStamp { get { return GetDetail<string>("SecurityStamp", null); } set { base.SetDetail<string>("SecurityStamp", value, null); } }

        #endregion

        [EditableChildren("Image sources", "Sources", 20)]
        public virtual IList<UserLogin> Logins
        {
            get
            {
                try
                {
                    var childItems = GetChildren();
                    if (childItems == null)
                        return new List<UserLogin>();
                    return childItems.Cast<UserLogin>();
                }
                catch (Exception ex)
                {
                    logger.Error("External login list", ex);
                    return new List<UserLogin>();
                }
            }
        }
    }
}