#region License
/* Copyright (C) 2007 Cristian Libardo
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 *
 * This software is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this software; if not, write to the Free
 * Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
 * 02110-1301 USA, or see the FSF site: http://www.fsf.org.
 */
#endregion

using System;
using System.Security.Principal;

namespace N2.Security
{
    /// <summary>
    /// This class defines roles or users authorized to access a certain item.
    /// </summary>
    public class AuthorizedRole: ICloneable
    {
        /// <summary>The role considered as everyone.</summary>
        public const string Everyone = "Everyone";
        /// <summary>Access to an anonymous user principal.</summary>
        public static IPrincipal AnonymousUser
        {
            get { return new GenericPrincipal(new GenericIdentity("Anonymous"), new string[] { "Everyone" }); }
        }

        #region Constructors
        /// <summary>Creates a new (empty) instance of the AuthorizedRole class.</summary>
        public AuthorizedRole()
        {
        }

        /// <summary>Creates a new instance of the AuthorizedRole class associating it with a content item and defining the role name.</summary>
        /// <param name="item">The item this role is associated with.</param>
        /// <param name="role">The role name.</param>
        public AuthorizedRole(ContentItem item, string role)
        {
            this.enclosingItem = item;
            this.role = role;
        }
        #endregion

        #region Private Fields
        private int id;
        private ContentItem enclosingItem;
        private string role;
        #endregion

        #region Public Properties
        /// <summary>Gets or sets the database identifier of this class.</summary>
        public virtual int ID
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>Gets or sets the item this AuthorizedRole referrs to.</summary>
		[System.Xml.Serialization.XmlIgnore]
		public virtual ContentItem EnclosingItem
        {
            get { return enclosingItem; }
            set { enclosingItem = value; }
        }

        /// <summary>Gets the role name this class referrs to.</summary>
        public virtual string Role
        {
            get { return role; }
            set { role = value; }
        }

        /// <summary>Gets wether this role referrs to everyone, i.e. the unauthenticated user.</summary>
        public virtual bool IsEveryone
        {
            get { return role == Everyone; }
        }

        #endregion

        /// <summary>Determines wether a user is permitted according to this role.</summary>
        /// <param name="user">The user to check.</param>
        /// <returns>True if the user is permitted.</returns>
        public virtual bool IsAuthorized(System.Security.Principal.IPrincipal user)
        {
            if (IsEveryone)
                return true;
            else if (user != null && user.IsInRole(Role))
                return true;
            return false;
        }

        #region ToString, Equals & GetHashCode
        /// <summary>Returns the role name associated with this class.</summary>
        /// <returns>The role name.</returns>
        public override string ToString()
        {
            return this.Role;
        }

        /// <summary>Comapres this role with another.</summary>
        /// <returns>True if the roles are the equal.</returns>
        public override bool Equals(object obj)
        {
            if (obj is string)
                return Role.Equals((string)obj);
            else if (obj is AuthorizedRole)
                return Role.Equals(((AuthorizedRole)obj).Role);
            else
                return base.Equals(obj);
        }

        /// <summary>Gets the hash code of the role name specified by this class.</summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            return role.GetHashCode();
        } 
        #endregion

        #region ICloneable Members

        /// <summary>Copies this AuthorizedRole clearing id and enclosing item.</summary>
        /// <returns>A copy of this AuthorizedRole.</returns>
        public virtual AuthorizedRole Clone()
        {
            AuthorizedRole cloned = (AuthorizedRole)this.MemberwiseClone();
            cloned.id = 0;
            cloned.enclosingItem = null;
            return cloned;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}
