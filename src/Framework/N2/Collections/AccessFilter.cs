using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using N2.Security;

namespace N2.Collections
{
    /// <summary>
    /// Filter based on user access and security.
    /// </summary>
    public class AccessFilter : ItemFilter
    {
        /// <summary>Used to decouple from HttpContext during testing.</summary>
        public static Func<IPrincipal> CurrentUser = () => HttpContext.Current != null ? HttpContext.Current.User : null;

        /// <summary>Used to decouple from N2.Context.Current during testing.</summary>
        public static Func<ISecurityManager> CurrentSecurityManager = () => Context.Current.SecurityManager;

        private IPrincipal user;
        private ISecurityManager securityManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessFilter"/> class.
        /// </summary>
        public AccessFilter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessFilter"/> class.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="securityManager">The security manager.</param>
        public AccessFilter(IPrincipal user, ISecurityManager securityManager)
        {
            this.user = user;
            this.securityManager = securityManager;
        }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>The user.</value>
        public IPrincipal User
        {
            get { return user ?? (user = CurrentUser()); }
            set { user = value; }
        }

        /// <summary>
        /// Gets or sets the security manager.
        /// </summary>
        /// <value>The security manager.</value>
        public ISecurityManager SecurityManager
        {
            get { return securityManager ?? (securityManager = CurrentSecurityManager()); }
            set { securityManager = value; }
        }

        /// <summary>
        /// Matches an item by checking if the user is authorised for the item, using the security manager.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool Match(ContentItem item)
        {
            return SecurityManager.IsAuthorized(item, User);
        }

        #region Static Methods

        /// <summary>
        /// Filters the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="user">The user.</param>
        /// <param name="securityManager">The security manager.</param>
        public static void Filter(IList<ContentItem> items, IPrincipal user, ISecurityManager securityManager)
        {
            Filter(items, new AccessFilter(user, securityManager));
        }

        /// <summary>
        /// Filters using the default filter.
        /// </summary>
        /// <param name="items">The items.</param>
        public static void DefaultFilter(IList<ContentItem> items)
        {
            Filter(items, new AccessFilter());
        }

        #endregion

        public override string ToString()
        {
            return "accessible";
        }
    }
}
