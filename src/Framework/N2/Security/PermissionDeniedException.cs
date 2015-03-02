using System.Security.Principal;
using System.Web;

namespace N2.Security
{
    /// <summary>
    /// An exeption thrown when a user tries to access an unauthorized item.
    /// </summary>
    public class PermissionDeniedException : HttpException
    {
        public PermissionDeniedException()
            : base(401, "Permission denied")
        {
        }

        public PermissionDeniedException(string message)
            : base(401, message)
        {
        }

        public PermissionDeniedException(ContentItem item)
            : base(401, "Permission denied")
        {
            this.item = item;
        }

        public PermissionDeniedException(ContentItem item, IPrincipal user)
            : this(401, "Permission denied", item, user)
        {
        }

        public PermissionDeniedException(int httpCode, string message, ContentItem item, IPrincipal user)
            : base (httpCode, message)
        {
            this.user = user;
            this.item = item;
        }

        public PermissionDeniedException(string message, PermissionDeniedException innerException)
            : base(401, message, innerException)
        {
            this.user = innerException.User;
            this.item = innerException.Item;
        }

        #region Private Members
        private ContentItem item;
        private IPrincipal user; 
        #endregion

        #region Properties
        /// <summary>Gets the user which caused the exception.</summary>
        public IPrincipal User
        {
            get { return user; }
        }

        /// <summary>Gets the item that caused the exception.</summary>
        public ContentItem Item
        {
            get { return item; }
        } 
        #endregion
    }
}
